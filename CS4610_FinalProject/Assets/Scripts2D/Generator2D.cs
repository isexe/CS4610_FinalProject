/* Adapted from https://github.com/vazgriz/DungeonGenerator

Copyright (c) 2019 Ryan Vazquez

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:


The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class Generator2D : MonoBehaviour {
    enum CellType {
        None,
        Room,
        Hallway
    }

    class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size) {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [Header("Generation Settings")]
    public int seed;
    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int roomCount;
    // max numb of rooms the algo will make
    public int maxNumRooms;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField]
    Vector2Int roomMinSize;
    
    [Header("Room Settings")]
    // room prefab stuff
    public float roomScale = 1;
    public GameObject roomPrefab; // should be a single unit of room
    public GameObject wallPrefab;
    public GameObject litWallPrefab;
    public GameObject chandelierPrefab; // used to light center of room

    // public GameObject roomEdgeTopPrefab; // lazy and need all 4 in inspector
    // public GameObject roomEdgeRightPrefab; // May remove bottom three and just rotate top one
    // public GameObject roomEdgeLeftPrefab;
    // public GameObject roomEdgeBottomPrefab;

    // public GameObject roomCornerTopLeftPrefab;  // same problem as room edges, lazy af and have 4 when need 1
    // public GameObject roomCornerTopRightPrefab;
    // public GameObject roomCornerBottomRightPrefab;
    // public GameObject roomCornerBottomLeftPrefab;

    // public GameObject hallPrefab; // should be a single unit of hall

    [Header("Spawners")]
    public GameObject playerSpawner;
    public GameObject enemySpawner;
    public GameObject bossSpawner;

    [Header("Demo Settings")]
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;

    Random random;
    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    private GameObject demoAnchor;
    private GameObject dungeonAnchor;

    void Start() {
        GameObject minimap = GameObject.FindGameObjectWithTag("SecondaryCamera");
        minimap.transform.position = new Vector3((size.x/2) * roomScale, 100, (size.y/2) * roomScale);

        demoAnchor = new GameObject("demoAnchor");
        dungeonAnchor = new GameObject("dungeonAnchor");

        Debug.Log("Generating Dungeon...");
        Generate();
        Debug.Log("...Finished!");
    }

    void Generate() {
        // Allows for customs seeded runs
        if(seed < 0) seed = new Random().Next();
        random = new Random(seed);

        grid = new Grid2D<CellType>(size, Vector2Int.zero);
        rooms = new List<Room>();

        Debug.Log("...Generating rooms...");
        PlaceRooms();

        Debug.Log("...Generating Triangulation...");
        Triangulate();

        Debug.Log("...Generating Hallways...");
        CreateHallways();

        Debug.Log("...Pathfinding Hallways...");
        PathfindHallways();

        Debug.Log("...Building Dungeon...");
        BuildDungeon();

        Debug.Log("...Populating Dungeon...");
        PopulateDungeon();
    }

    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                random.Next(0, size.x),
                random.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
            // breaks if room exceeds a certain number
            if(rooms.Count >= maxNumRooms) break;
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
        // BuildDemoDelaunay();
    }

    void BuildDemoDelaunay(){
        foreach(var edge in delaunay.Edges){
            Vector3 pt1 = new Vector3(edge.U.Position.x * roomScale, 0, edge.U.Position.y * roomScale);
            Vector3 pt2 = new Vector3(edge.V.Position.x * roomScale, 0, edge.V.Position.y * roomScale);

            Debug.DrawLine(pt1, pt2, Color.cyan, Mathf.Infinity, false);
        }
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);
        

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);
        
        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.125) {
                selectedEdges.Add(edge);
            }
        }

        BuildDemoHallways();
    }

    void BuildDemoHallways(){
        foreach(var edge in selectedEdges){
            Vector3 pt1 = new Vector3(edge.U.Position.x * roomScale, 0, edge.U.Position.y * roomScale);
            Vector3 pt2 = new Vector3(edge.V.Position.x * roomScale, 0, edge.V.Position.y * roomScale);

            Debug.DrawLine(pt1, pt2, Color.cyan, Mathf.Infinity, false);
        }
    }

    void PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room) {
                    pathCost.cost += 10;
                } else if (grid[b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway) {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material) {
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x * roomScale, 10, location.y * roomScale), Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * roomScale, 1, size.y * roomScale);
        go.GetComponent<MeshRenderer>().material = material;

        go.transform.SetParent(demoAnchor.transform, true);
    }

    void PlaceRoom(Vector2Int location, Vector2Int size) {
        PlaceCube(location, size, redMaterial);
    }

    void PlaceHallway(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), blueMaterial);
    }

    void BuildDungeon(){
        for(int x=0; x<grid.Size.x; x++){
            for(int y=0; y<grid.Size.y; y++){
                CellType cell = grid[x,y];
                switch(cell){
                    case CellType.Room:
                        BuildRoom(x,y);
                        break;

                    case CellType.Hallway:
                        BuildHallway(x,y);
                        break;
                }
            }
        }

        foreach(var room in rooms){
            // hard code height of chandelier
            // could just set y in prefab and use that but for now this good
            Vector3 pos = new Vector3(room.bounds.center.x * roomScale, 4.6f, room.bounds.center.y * roomScale);
            GameObject temp = Instantiate<GameObject>(chandelierPrefab, pos, chandelierPrefab.transform.rotation);
            temp.transform.SetParent(dungeonAnchor.transform);
        }
    }

    void BuildHallway(int x, int y){
        GameObject temp = Instantiate<GameObject>(roomPrefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), Quaternion.identity);
        temp.transform.SetParent(dungeonAnchor.transform, true);

        GameObject prefab = wallPrefab;
        if(random.NextDouble() < 0.25) prefab = litWallPrefab; 

        //check walls
        // for each cell next to current check...
        //      if edge end of grid, if so build wall
        //      else if edge is empty, if so build wall
        if(y-1 < 0){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            //wallTmp.transform.Rotate(new Vector3(0,0,0));  //prefab facing towards north so don't need this
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x,y-1] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            //wallTmp.transform.Rotate(new Vector3(0,0,0));  //prefab facing towards north so don't need this
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        
        if(y+1>grid.Size.y){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,180,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        } 
        else if(grid[x,y+1] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,180,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }

        if(x-1 < 0){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,90,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x-1,y] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,90,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        
        if(x+1>grid.Size.x){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,270,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x+1,y] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,270,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
    }

    void BuildRoom(int x, int y){
        // build room
        GameObject temp = Instantiate<GameObject>(roomPrefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), roomPrefab.transform.rotation);
        temp.transform.SetParent(dungeonAnchor.transform, true);

        GameObject prefab = wallPrefab;
        if(random.NextDouble() < 0.1) prefab = litWallPrefab;

        //check walls
        // for each cell next to current check...
        //      if edge end of grid, if so build wall
        //      else if edge is empty, if so build wall
        if(y-1 < 0){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            //wallTmp.transform.Rotate(new Vector3(0,0,0));  //prefab facing towards north so don't need this
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x,y-1] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            //wallTmp.transform.Rotate(new Vector3(0,0,0));  //prefab facing towards north so don't need this
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }

        if(y+1>grid.Size.y){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,180,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        } 
        else if(grid[x,y+1] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,180,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }

        if(x-1 < 0){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,90,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x-1,y] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,90,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }

        if(x+1>grid.Size.x){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,270,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
        else if(grid[x+1,y] == CellType.None){
            GameObject wallTmp = Instantiate<GameObject>(prefab, new Vector3(x*roomScale + roomScale/2, 0, y*roomScale + roomScale/2), prefab.transform.rotation);
            wallTmp.transform.Rotate(new Vector3(0,270,0));
            wallTmp.transform.SetParent(dungeonAnchor.transform, true);
        }
    }

    void PopulateDungeon(){
        List<Room> roomsLeft = new List<Room>(rooms);
        Room bossRoom;
        Room spawnRoom;

        List<Room> tempRooms = new List<Room>();
        foreach(var edge in selectedEdges){
            tempRooms.Add((edge.U as Vertex<Room>).Item);
            tempRooms.Add((edge.V as Vertex<Room>).Item);
        }
        List<Room> possibleRooms = new List<Room>();
        foreach(var room in roomsLeft){
            int count = 0;
            foreach(var tempRoom in tempRooms){
                if(room == tempRoom) count += 1;
            }
            if(count == 1){
                possibleRooms.Add(room);
            }
        }
        if(possibleRooms.Count > 0) bossRoom = possibleRooms[random.Next(possibleRooms.Count)];
        else bossRoom = rooms[0]; // backup incase there is not a single room
        roomsLeft.Remove(bossRoom);

        spawnRoom = roomsLeft[random.Next(roomsLeft.Count)];
        roomsLeft.Remove(spawnRoom);
        
        GameObject temp; 
        // Spawn boss
        temp = Instantiate<GameObject>(bossSpawner);
        temp.transform.position = new Vector3(bossRoom.bounds.center.x * roomScale, 0, bossRoom.bounds.center.y * roomScale);
        temp.transform.SetParent(dungeonAnchor.transform);

        // Spawn enemies
        foreach(var room in roomsLeft){
            temp = Instantiate<GameObject>(enemySpawner);
            temp.transform.position = new Vector3(room.bounds.center.x * roomScale, 0, room.bounds.center.y * roomScale);
            temp.transform.SetParent(dungeonAnchor.transform);
        }
        // Spawn player
        temp = Instantiate<GameObject>(playerSpawner);
        temp.transform.position = new Vector3(spawnRoom.bounds.center.x * roomScale, 0, spawnRoom.bounds.center.y * roomScale);
        temp.transform.SetParent(dungeonAnchor.transform);


        // Debug stuff for rooms
        temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "BossRoom";
        temp.GetComponent<Renderer>().material.color = Color.black;
        temp.transform.position = new Vector3(bossRoom.bounds.center.x * roomScale, 25, bossRoom.bounds.center.y * roomScale);
        temp.transform.localScale = temp.transform.localScale * 5;
        temp.transform.SetParent(demoAnchor.transform);

        temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "SpawnRoom";
        temp.GetComponent<Renderer>().material.color = Color.blue;
        temp.transform.position = new Vector3(spawnRoom.bounds.center.x * roomScale, 25, spawnRoom.bounds.center.y * roomScale);
        temp.transform.localScale = temp.transform.localScale * 5;
        temp.transform.SetParent(demoAnchor.transform);

        foreach(var room in roomsLeft){
            temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.name = "EnemyRoom";
            temp.GetComponent<Renderer>().material.color = Color.red;
            temp.transform.position = new Vector3(room.bounds.center.x * roomScale, 25, room.bounds.center.y * roomScale);
            temp.transform.localScale = temp.transform.localScale * 5;
            temp.transform.SetParent(demoAnchor.transform);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    // DataType for rooms
    // Need one for empty, room, and hallway
    // May add one for walls but (x) to doubt
     public enum DataType
    {
        None,
        Room,
        Hall
    }

    /// <summary>
    /// Room class for storing info
    /// Contains the bounds of the room
    ///     This is composed of position and size
    /// Has function to detect intersection between rooms
    /// </summary>
    public class Room
    {
        public RectInt bounds;

        // contructor
        // might add another that takes minX, maxX, minY, maxY as para
        public Room(Vector2Int position, Vector2Int size)
        {
            bounds = new RectInt(position, size);
        }

        //public  bool Intersect(Room r1, Room r2)
        //{
        //    return !((r1.bounds.position.x >= (r2.bounds.position.x + r2.bounds.size.x)) || ((r1.bounds.position.x + r1.bounds.size.x) <= r2.bounds.position.x)
        //        || (r1.bounds.position.y >= (r2.bounds.position.y + r2.bounds.size.y)) || ((r1.bounds.position.y + r1.bounds.size.y) <= r2.bounds.position.y));
        //}

        // Checks if this room intersects other room
        public bool Intersect(Room other)
        {
            return (this.bounds.xMin <= other.bounds.xMax && other.bounds.xMin <= this.bounds.xMax
            && this.bounds.yMin <= other.bounds.yMax && other.bounds.yMin <= this.bounds.yMax);
        }
    }

    // map settings
    [HideInInspector]  // currently map origin doesn't function properly so we don't fuck with it
    public Vector2Int mapOrigin = new Vector2Int(0,0);  // bottom left corner of map? idrk
    
    [Header("Map Settings")]
    public Vector2Int mapSize;
    public int roomGenAttempts; // pretty much just used during testing to stop inf loops
    public int maxNumOfRooms;   // helps reduce clutter
    public Vector2Int roomMaxSize;
    public Vector2Int roomMinSize;
    public float roomScale = 1;

    // prefabs
    [Header("Prefabs")]
    public GameObject demoPrefab; // not really used
    public GameObject roomPrefab; // should be a single unit of room

    public GameObject chandelierPrefab; // used to light center of room

    public GameObject roomEdgeTopPrefab; // lazy and need all 4 in inspector
    public GameObject roomEdgeRightPrefab; // May remove bottom three and just rotate top one
    public GameObject roomEdgeLeftPrefab;
    public GameObject roomEdgeBottomPrefab;

    public GameObject roomCornerTopLeftPrefab;  // same problem as room edges, lazy af and have 4 when need 1
    public GameObject roomCornerTopRightPrefab;
    public GameObject roomCornerBottomRightPrefab;
    public GameObject roomCornerBottomLeftPrefab;

    public GameObject hallPrefab; // should be a single unit of hall

    // used for seeded random gen
    // may try to implement seeded runs like Binding of Isaac
    Random random;
    int seed;

    // data for gen
    Grid2D<DataType> grid;
    List<Room> rooms;
    Graph graph;

    // anchors for hierarchy
    GameObject roomAnchor;

    void Start()
    {
        // Create line boundary for debugging
        LineRenderer bounds = gameObject.GetComponent<LineRenderer>();
        if(bounds != null)
        {
            bounds.positionCount = 4;
            bounds.SetPosition(0, new Vector3(mapOrigin.x * roomScale, 0, mapOrigin.y * roomScale) + transform.position * roomScale);
            bounds.SetPosition(1, new Vector3((mapOrigin.x + mapSize.x) * roomScale, 0, mapOrigin.y * roomScale) + transform.position * roomScale);
            bounds.SetPosition(2, new Vector3((mapOrigin.x + mapSize.x) * roomScale, 0, (mapOrigin.y + mapSize.y) * roomScale) + transform.position * roomScale);
            bounds.SetPosition(3, new Vector3(mapOrigin.x * roomScale, 0, (mapOrigin.y + mapSize.y) * roomScale) + transform.position * roomScale);
        }

        // Begin hell
        Debug.Log("Generating Map...");
        GenerateMap();
        Debug.Log("...Finished :)");
        // Hope nothing's fucked
        
        // Draw Demo Grid for debugging
        CreateGridDemo();
        CreateDungeon();
        CreateGraphDemo();
        
        // *Used to debug the triangle calculations, they currently work
        //Triangle triangle = new Triangle(new Vector2(4, 5), new Vector2(6, 8), new Vector2(5, -2));
        //Vector2 center = triangle.CalculateCircumcenter();
        //float radius = triangle.CalculateCircumradius(center);

        //Debug.Log(center);
        //Debug.Log(radius);

        //Debug.DrawLine(new Vector3(4, 0, 5), new Vector3(6, 0, 8), Color.magenta, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(6, 0, 8), new Vector3(5, 0, -2), Color.magenta, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(5, 0, -2), new Vector3(4, 0, 5), Color.magenta, Mathf.Infinity, false);
        
        //Debug.DrawLine(new Vector3(4, 0, 5), new Vector3(center.x, 0, center.y), Color.red, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(6, 0, 8), new Vector3(center.x, 0, center.y), Color.red, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(5, 0, -2), new Vector3(center.x, 0, center.y), Color.red, Mathf.Infinity, false);

        //Debug.DrawLine(new Vector3(center.x-radius, 0, center.y), new Vector3(center.x, 0, center.y), Color.cyan, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(center.x + radius, 0, center.y), new Vector3(center.x, 0, center.y), Color.cyan, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(center.x, 0, center.y-radius), new Vector3(center.x, 0, center.y), Color.cyan, Mathf.Infinity, false);
        //Debug.DrawLine(new Vector3(center.x, 0, center.y + radius), new Vector3(center.x, 0, center.y), Color.cyan, Mathf.Infinity, false);
    }

    void GenerateMap()
    {
        seed = new Random().Next();
        Debug.Log("Seed: " + seed);
        random = new Random(seed);
        grid = new Grid2D<DataType>(mapSize, mapOrigin);
        rooms = new List<Room>(maxNumOfRooms);

        // TODO Generation "works", need to build rooms after completion
        // beings room generation
        Debug.Log("...Generating Rooms...");
        GenerateRooms();

        //// TODO
        //Debug.Log("...Generating Hallways...");
        //GenerateHallways();

        //// TODO
        //Debug.Log("...Generating a Boss Room...");
        //GenerateBossRoom();

        //// TODO
        //Debug.Log("...Generating a Reward Room...");
        //GenerateRewardRoom();

        //// TODO
        //Debug.Log("...Populating Empty Rooms with Enemies...");
        //GenerateEnemyRoom();
    }

    void GenerateRooms()
    {
        // will attempt to make room up to max count
        for (int attempt = 0; attempt < roomGenAttempts; attempt++)
        {
            // generate random room position
            Vector2Int tPos = new Vector2Int(
                random.Next(mapOrigin.x, mapSize.x),
                random.Next(mapOrigin.y, mapSize.y)
            );

            // generate random room size
            Vector2Int tSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y + 1)
            );

            // make random room
            Room tRoom = new Room(tPos, tSize);

            // !Maybe useless if my intersect function works - keep just incase
            // buffer for rooms
            // used to ensure no two rooms are next to one another
            //Room roomBuffer = new Room(
            //    tPos + new Vector2Int(-1,-1),
            //    tSize + new Vector2Int(2,2)
            //);

            //check for validity
            bool valid = true;
            // makes sure room in bounds
            if ((tRoom.bounds.xMax >= mapSize.x || tRoom.bounds.xMin < mapOrigin.x)
            || (tRoom.bounds.yMax >= mapSize.y || tRoom.bounds.yMin < mapOrigin.y))
            {
                valid = false;
            }
            
            // !Currently doesn't work, use collider for detection for now
            else
            {
                //makes sure tRoom doesn't intersect with other rooms
                foreach (Room r in rooms)
                {
                    //if (Room.Intersect(r, roomBuffer))
                    //{
                    //    valid = false;
                    //    break;
                    //}

                    if (tRoom.Intersect(r))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            // if valid build room
            if (valid)
            {
                rooms.Add(tRoom);
                foreach(var pos in tRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = DataType.Room;
                    // TODO Either build room unit here or after entire algo
                }
            }

            // break if maxNumOfRooms have been generated
            if(rooms.Count >= maxNumOfRooms)
            {
                break;
            }
        }
    }
    void CreatePrefab(Vector2Int position, GameObject gameObject, GameObject anchor = null)
    {
        GameObject cell = Instantiate<GameObject>(gameObject);
        cell.transform.position = new Vector3(position.x * roomScale, 0, position.y * roomScale);
        cell.transform.localScale = new Vector3(1, 1, 1);
        if(anchor != null) cell.transform.SetParent(anchor.transform, true);
    }

    // Creates an empty dungeon room at each room object
    void CreateDungeon()
    {
        
        roomAnchor = GameObject.Find("anchorRooms");
        if (roomAnchor == null)
        {
            roomAnchor = new GameObject("anchorDungeon");
            roomAnchor.transform.SetParent(gameObject.transform);
        }

        int count = 0;
        foreach (Room r in rooms)
        {
            count += 1;
            CreateRoom(r, count);
        }
    }

    void CreateRoom(Room r, int num)
    {
        // Creates anchor for room components
        GameObject tempAnchor = roomAnchor;
        roomAnchor = new GameObject("Room_" + num);

        CreateRoomCenter(r, roomAnchor);
        CreateRoomHorizontalEdges(r, roomAnchor);
        CreateRoomVerticalEdges(r, roomAnchor);
        CreateRoomCorners(r, roomAnchor);


        // TODO maybe generate chandelier at the center of room?
        // !Hard coded height of chandelier, if issues with placement check that
        //Debug.Log(r.bounds.center);
        GameObject chandelier = Instantiate<GameObject>(chandelierPrefab, new Vector3((r.bounds.center.x - .5f) * roomScale, 4.35f, (r.bounds.center.y - .5f) * roomScale), Quaternion.identity);
        chandelier.transform.SetParent(roomAnchor.transform);

        // Sets room anchor to parent anchor
        roomAnchor.transform.SetParent(tempAnchor.transform);
        roomAnchor = tempAnchor;
    }

    void CreateRoomCenter(Room r, GameObject anchor = null)
    {
        for(int x = r.bounds.xMin + 1; x<r.bounds.xMax - 1; x++)
        {
            for(int y = r.bounds.yMin + 1; y<r.bounds.yMax - 1; y++)
            {
                CreatePrefab(new Vector2Int(x, y), roomPrefab, anchor);
            }
        }
    }

    void CreateRoomHorizontalEdges(Room r, GameObject anchor = null)
    {
        for(int x=r.bounds.xMin + 1; x<r.bounds.xMax - 1; x++)
        {
            // Bottom edge of room
            CreatePrefab(new Vector2Int(x, r.bounds.yMin), roomEdgeBottomPrefab, anchor);
            // Top edge of room
            CreatePrefab(new Vector2Int(x, r.bounds.yMax-1), roomEdgeTopPrefab, anchor);
        }
    }
    void CreateRoomVerticalEdges(Room r, GameObject anchor = null)
    {
        for (int y = r.bounds.yMin + 1; y < r.bounds.yMax - 1; y++)
        {
            // Left Edge of room
            CreatePrefab(new Vector2Int(r.bounds.xMin, y), roomEdgeLeftPrefab, anchor);
            // Right Edge of room
            CreatePrefab(new Vector2Int(r.bounds.xMax-1, y), roomEdgeRightPrefab, anchor);
        }
    }

    void CreateRoomCorners(Room r, GameObject anchor = null)
    {
        CreatePrefab(new Vector2Int(r.bounds.xMin, r.bounds.yMin), roomCornerBottomLeftPrefab, anchor);
        CreatePrefab(new Vector2Int(r.bounds.xMax-1, r.bounds.yMin), roomCornerBottomRightPrefab, anchor);
        CreatePrefab(new Vector2Int(r.bounds.xMin, r.bounds.yMax-1), roomCornerTopLeftPrefab, anchor);
        CreatePrefab(new Vector2Int(r.bounds.xMax-1, r.bounds.yMax-1), roomCornerTopRightPrefab, anchor);
    }

    void GenerateHallways()
    {

    }

    void GenerateBossRoom()
    {

    }

    void GenerateRewardRoom()
    {

    }

    void GenerateEnemyRoom()
    {

    }

    // Works for demoing grid
    void CreateGridDemo()
    {
        GameObject anchor;
        anchor = GameObject.Find("anchorGrid");
        if (anchor == null) anchor = new GameObject("anchorGrid");

        for (int x=0; x<grid.Size.x; x++)
        {
            for(int y=0; y<grid.Size.y; y++)
            {
                DataType type = grid[x,y];
                switch (type)
                {
                    case DataType.Room:
                        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cell.transform.position = new Vector3(x * roomScale, -1, y * roomScale);

                        cell.transform.localScale = new Vector3(1 * roomScale, 1, 1 * roomScale);
                        cell.transform.SetParent(anchor.transform, true);
                        break;
                    // TODO add extra stuff for hallways later
                }
            }
        }

        anchor.transform.SetParent(transform);
    }

    void CreateGraphDemo()
    {
        graph = new Graph();

        foreach (Room r in rooms)
        {
            Vector2 point = r.bounds.center - new Vector2(.5f, .5f);
            graph.AddPoint(point);
        }

        //graph.GenerateDemoEdges();

        graph.DelanuayTriangluation();

        DrawGraph();
    }

    public void DrawGraph()
    {
        foreach (Edge e in graph.Edges)
        {
            Vector3 pt1 = new Vector3(e.P1.X * roomScale, 0, e.P1.Y * roomScale);
            Vector3 pt2 = new Vector3(e.P2.X * roomScale, 0, e.P2.Y * roomScale);

            Debug.DrawLine(pt1, pt2, Color.cyan, Mathf.Infinity, false);
        }
    }
}

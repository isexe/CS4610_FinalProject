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
    public float sizeOfUnit = 1;

    // prefabs
    [Header("Prefabs")]
    public GameObject demoPrefab; // not really used
    public GameObject roomPrefab; // should be a single unit of room

    public GameObject roomTopEdgePrefab; // lazy and need all 4 in inspector
    public GameObject roomRightEdgePrefab; // May remove bottom three and just rotate top one
    public GameObject roomLeftEdgePrefab;
    public GameObject roomBottomEdgePrefab;

    public GameObject roomTopLeftCornerPrefab;  // same problem as room edges, lazy af and have 4 when need 1
    public GameObject roomTopRightCornerPrefab;
    public GameObject roomBottomRightCornerPrefab;
    public GameObject roomBottomLeftCornerPrefab;

    public GameObject hallPrefab; // should be a single unit of hall

    // used for seeded random gen
    // may try to implement seeded runs like Binding of Isaac
    Random random;
    int seed;

    // data for gen
    Grid2D<DataType> grid;
    List<Room> rooms;

    // anchor for GO
    GameObject anchor;

    void Start()
    {
        // Create line boundary for debugging
        LineRenderer bounds = gameObject.GetComponent<LineRenderer>();
        if(bounds != null)
        {
            bounds.positionCount = 4;
            bounds.SetPosition(0, new Vector3(mapOrigin.x * sizeOfUnit, 0, mapOrigin.y * sizeOfUnit) + transform.position * sizeOfUnit);
            bounds.SetPosition(1, new Vector3((mapOrigin.x + mapSize.x) * sizeOfUnit, 0, mapOrigin.y * sizeOfUnit) + transform.position * sizeOfUnit);
            bounds.SetPosition(2, new Vector3((mapOrigin.x + mapSize.x) * sizeOfUnit, 0, (mapOrigin.y + mapSize.y) * sizeOfUnit) + transform.position * sizeOfUnit);
            bounds.SetPosition(3, new Vector3(mapOrigin.x * sizeOfUnit, 0, (mapOrigin.y + mapSize.y) * sizeOfUnit) + transform.position * sizeOfUnit);
        }

        // Begin hell
        Debug.Log("Generating Map...");
        GenerateMap();
        Debug.Log("...Finished :)");
        // Hope nothing's fucked
        
        // Draw Demo Grid for debugging
        CreateGridDemo();
        CreateDungeon();
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

        // TODO
        Debug.Log("...Generating Hallways...");
        GenerateHallways();

        // TODO
        Debug.Log("...Generating a Boss Room...");
        GenerateBossRoom();

        // TODO
        Debug.Log("...Generating a Reward Room...");
        GenerateRewardRoom();

        // TODO
        Debug.Log("...Populating Empty Rooms with Enemies...");
        GenerateEnemyRoom();
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

    // Doesn't work well, everything is scaled wrong and looks messy af
    void CreateDungeon()
    {
        anchor = GameObject.Find("anchorDungeon");
        if (anchor == null) anchor = new GameObject("anchorDungeon");

        foreach (Room r in rooms)
        {
            CreateRoom(r);

            //for(int x=r.bounds.xMin; x<=r.bounds.xMax; x++)
            //{
            //    CreateRoomBottomEdge(new Vector2Int(x, r.bounds.yMin));
            //}
            //for (int x = r.bounds.xMin; x <= r.bounds.xMax; x++)
            //{
            //    CreateRoomBottomEdge(new Vector2Int(x, r.bounds.yMin));
            //}
            //for (int x = r.bounds.xMin; x <= r.bounds.xMax; x++)
            //{
            //    CreateRoomBottomEdge(new Vector2Int(x, r.bounds.yMin));
            //}

            CreateRoomHorizontalEdges(r);
            CreateRoomVerticalEdges(r);
            CreateRoomCorners(r);
        }
    }

    void CreateGameObject(Vector2Int position, GameObject gameObject)
    {
        GameObject cell = Instantiate<GameObject>(gameObject);
        cell.transform.position = new Vector3(position.x * sizeOfUnit, 1, position.y * sizeOfUnit);
        cell.transform.localScale = new Vector3(1, 1, 1);
        cell.transform.SetParent(anchor.transform, true);
    }

    void CreateRoom(Room r)
    {
        for(int x = r.bounds.xMin + 1; x<r.bounds.xMax - 1; x++)
        {
            for(int y = r.bounds.yMin + 1; y<r.bounds.yMax - 1; y++)
            {
                CreateGameObject(new Vector2Int(x, y), roomPrefab);
            }
        }
        
    }

    void CreateRoomHorizontalEdges(Room r)
    {
        for(int x=r.bounds.xMin + 1; x<r.bounds.xMax - 1; x++)
        {
            CreateGameObject(new Vector2Int(x, r.bounds.yMin), roomBottomEdgePrefab);
            CreateGameObject(new Vector2Int(x, r.bounds.yMax-1), roomTopEdgePrefab);
        }
    }
    void CreateRoomVerticalEdges(Room r)
    {
        for (int y = r.bounds.yMin + 1; y < r.bounds.yMax - 1; y++)
        {
            CreateGameObject(new Vector2Int(r.bounds.xMin, y), roomLeftEdgePrefab);
            CreateGameObject(new Vector2Int(r.bounds.xMax-1, y), roomRightEdgePrefab);
        }
    }

    void CreateRoomCorners(Room r)
    {

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
                        cell.transform.position = new Vector3(x * sizeOfUnit, -1, y * sizeOfUnit);

                        cell.transform.localScale = new Vector3(1 * sizeOfUnit, 1, 1 * sizeOfUnit);
                        cell.transform.SetParent(anchor.transform, true);
                        break;
                    // TODO add extra stuff for hallways later
                }
            }
        }
    }
}

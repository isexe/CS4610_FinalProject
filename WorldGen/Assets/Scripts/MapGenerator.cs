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
    [Header("Map Settings")]
    [HideInInspector]  // currently map origin doesn't function properly so we don't fuck with it
    public Vector2Int mapOrigin = new Vector2Int(0,0);  // bottom left corner of map? idrk
    public Vector2Int mapSize;
    public int roomGenAttempts; // pretty much just used during testing to stop inf loops
    public int maxNumOfRooms;   // helps reduce clutter
    public Vector2Int roomMaxSize;
    public Vector2Int roomMinSize;

    // prefabs
    public GameObject demoPrefab; // not really used
    public GameObject roomPrefab; // should be a single unit of room
    public GameObject hallPrefab; // should be a single unit of hall

    // used for seeded random gen
    // may try to implement seeded runs like Binding of Isaac
    Random random;
    int seed;

    // data for gen
    Grid2D<DataType> grid;
    List<Room> rooms;

    void Start()
    {
        // Create line boundary for debugging
        LineRenderer bounds = gameObject.GetComponent<LineRenderer>();
        if(bounds != null)
        {
            bounds.positionCount = 4;
            bounds.SetPosition(0, new Vector3(mapOrigin.x, 0, mapOrigin.y) + transform.position);
            bounds.SetPosition(1, new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.y) + transform.position);
            bounds.SetPosition(2, new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.y + mapSize.y) + transform.position);
            bounds.SetPosition(3, new Vector3(mapOrigin.x, 0, mapOrigin.y + mapSize.y) + transform.position);
        }

        // Begin hell
        Debug.Log("Generating Map...");
        GenerateMap();
        Debug.Log("...Finished :)");
        // Hope nothing's fucked
        
        // Draw Demo Grid for debugging
        CreateGridDemo();
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
    void CreateRoomDemo(Vector2Int position, Vector2Int size, Color color, float height = 0)
    {
        // create and set room
        GameObject tGO = Instantiate<GameObject>(demoPrefab);
        tGO.transform.position = new Vector3(position.x, height, position.y);
        tGO.transform.localScale = new Vector3(size.x, 1, size.y);
        tGO.GetComponent<Renderer>().material.color = color;

        //Create anchor to house rooms
        GameObject anchor;
        anchor = GameObject.Find("anchorMap");
        if (anchor == null) anchor = new GameObject("anchorMap");
        tGO.transform.SetParent(anchor.transform, true);
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
        
        for(int x=0; x<grid.Size.x; x++)
        {
            for(int y=0; y<grid.Size.y; y++)
            {
                DataType type = grid[x,y];
                switch (type)
                {
                    case DataType.Room:
                        GameObject cell = Instantiate<GameObject>(demoPrefab);
                        cell.transform.position = new Vector3(x, 1, y);
                        cell.transform.SetParent(anchor.transform, true);
                        break;
                    // TODO add extra stuff for hallways later
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    public enum DataType
    {
        None,
        Room,
        Hallway
    }

    public class Room
    {
        public RectInt bounds;

        public Room(Vector2Int position, Vector2Int size)
        {
            bounds = new RectInt(position, size);
        }

        // !This shit straightup broken
        // public bool Intersect(Room other)
        // {
        //     // TODO create intersection check
        //     return bounds.Overlaps(other.bounds); //placeholder
        // }
        public static bool Intersect(Room r1, Room r2)
        {
            return !((r1.bounds.position.x >= (r2.bounds.position.x + r2.bounds.size.x)) || ((r1.bounds.position.x + r1.bounds.size.x) <= r2.bounds.position.x)
                || (r1.bounds.position.y >= (r2.bounds.position.y + r2.bounds.size.y)) || ((r1.bounds.position.y + r1.bounds.size.y) <= r2.bounds.position.y));
        }
    }

    // map settings
    public Vector2Int mapOrigin;
    public Vector2Int mapSize;
    public int roomMaxCount;
    public Vector2Int roomMaxSize;
    public Vector2Int roomMinSize;

    // prefabs
    public GameObject demoPrefab;
    public GameObject roomPrefab;
    public GameObject hallPrefab;

    // used for seeded random gen
    Random random;
    int seed;

    // data for gen
    Grid2D<DataType> grid;
    List<Room> rooms;

    void Start()
    {
        LineRenderer bounds = gameObject.GetComponent<LineRenderer>();
        if(bounds != null)
        {
            bounds.positionCount = 4;
            bounds.SetPosition(0, Vector3.zero);
            bounds.SetPosition(1, new Vector3(mapSize.x, 0, 0));
            bounds.SetPosition(2, new Vector3(mapSize.x, 0, mapSize.y));
            bounds.SetPosition(3, new Vector3(0, 0, mapSize.y));
        }

        Debug.Log("Generating Map...");
        GenerateMap();
        Debug.Log("...Finished :)");
        CreateGridDemo();
    }

    void GenerateMap()
    {
        random = new Random(0);
        grid = new Grid2D<DataType>(mapSize, Vector2Int.zero);
        rooms = new List<Room>(roomMaxCount);

        // TODO
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
        for (int attempt = 0; attempt < roomMaxCount; attempt++)
        {
            // generate random room position
            Vector2Int tPos = new Vector2Int(
                random.Next(0, mapSize.x),
                random.Next(0, mapSize.y)
            );

            // generate random room size
            Vector2Int tSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMinSize.y + 1)
            );

            // make random room
            Room tRoom = new Room(tPos, tSize);

            // buffer for rooms
            // used to ensure no two rooms are next to one another
            Room roomBuffer = new Room(
                tPos,
                tSize + new Vector2Int(2,2)
            );

            //check for validity
            bool valid = true;
            // makes sure room in bounds
            if ((tRoom.bounds.xMax >= mapSize.x || tRoom.bounds.xMin < 0)
            || (tRoom.bounds.yMax >= mapSize.y || tRoom.bounds.yMin < 0))
            {
                valid = false;
            }
            
            // !Currently doesn't work, use collider for detection for now
            else
            {
                // makes sure rooms don't intersect
                foreach (Room r in rooms)
                {
                    if (Room.Intersect(r, roomBuffer))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            // if valid build room
            if (valid)
            {
                // TODO create room
                // TODO CreateRoom(Room room);
                // currently used to debug
                // This doesn't even work either :(
                rooms.Add(tRoom);

                // Create buffer around room for debugging
                //CreateRoomDemo(roomBuffer.bounds.position, roomBuffer.bounds.size, Color.black);
                CreateRoomDemo(tRoom.bounds.position, tRoom.bounds.size, Color.blue);

                foreach(var pos in tRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = DataType.Room;
                }

            }
        }
    }

    void CreateRoomDemo(Vector2Int position, Vector2Int size, Color color)
    {
        // create and set room
        GameObject tGO = Instantiate<GameObject>(demoPrefab);
        tGO.transform.position = new Vector3(position.x, 0, position.y);
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
                        cell.transform.position = new Vector3(x, -1, y);
                        cell.transform.SetParent(anchor.transform, true);
                        break;
                    // TODO add extra stuff for hallways later
                }
            }
        }
    }
}

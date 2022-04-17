using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DataType
    {
        None,
        Room,
        Hall
    }

    public class Room
    {
        public GameObject roomGO;
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
        // public bool Intersect(Room b) {
        //     return !((this.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((this.bounds.position.x + this.bounds.size.x) <= b.bounds.position.x)
        //         || (this.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((this.bounds.position.y + this.bounds.size.y) <= b.bounds.position.y));
        // }
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

    // data for algo
    Grid2D<DataType> grid;
    List<Room> rooms;

    void Start()
    {
        Debug.Log("Generating Map...");
        GenerateMap();
        Debug.Log("...Finished :)");
    }

    void GenerateMap()
    {
        grid = new Grid2D<DataType>(mapSize, mapOrigin);
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
            int xPos = Random.Range(mapOrigin.x, mapSize.x);
            int yPos = Random.Range(mapOrigin.y, mapSize.y);
            Vector2Int tPos = new Vector2Int(xPos, yPos);

            // generate random room size
            int xSize = Random.Range(roomMinSize.x, roomMaxSize.x);
            int ySize = Random.Range(roomMinSize.y, roomMaxSize.y);
            Vector2Int tSize = new Vector2Int(xSize, ySize);

            // make random room
            Room tRoom = new Room(tPos, tSize);

            // buffer for rooms
            // used to ensure no two rooms are next to one another
            Vector2Int bufferPos = new Vector2Int(-1, -1);
            Vector2Int bufferSize = new Vector2Int(2, 2);
            Room roomBuffer = new Room(tPos + bufferPos, tSize + bufferSize);

            //check for validity
            bool valid = true;
            // makes sure room in bounds
            if ((tRoom.bounds.xMax >= mapSize.x || tRoom.bounds.xMin < mapOrigin.x)
            || (tRoom.bounds.yMax >= mapSize.y || tRoom.bounds.yMin < mapOrigin.y))
            {
                valid = false;
            }
            // !Currently doesn't work, use collider for detection for now
            // else
            // {
            //     // makes sure rooms don't intersect
            //     foreach (Room r in rooms)
            //     {
            //         if (roomBuffer.Intersect(r))
            //         {
            //             valid = false;
            //             break;
            //         }
            //     }
            // }

            // if valid build room
            if (valid)
            {
                // TODO create room
                // TODO CreateRoom(Room room);
                // currently used to debug
                // This doesn't even work either :(
                CreateRoomDemo(roomBuffer);
                float timer = 0;
                while(timer < 1) timer += Time.deltaTime;

                // add room to rooms list
                if(roomBuffer.roomGO != null)
                {
                    Destroy(roomBuffer.roomGO.gameObject);
                    CreateRoomDemo(tRoom);
                    tRoom.roomGO.gameObject.tag = "Room";
                    rooms.Add(tRoom);
                    // add room to grid
                    foreach (var pos in tRoom.bounds.allPositionsWithin)
                    {
                        grid[pos] = DataType.Room;
                    }
                }
            }
        }
    }

    void CreateRoomDemo(Room room)
    {
        // find tranform for room
        Vector3 pos = new Vector3(room.bounds.position.x, 0, room.bounds.position.y);
        Vector3 scale = new Vector3(room.bounds.size.x, 1, room.bounds.size.y);

        // create and set room
        GameObject tGO = Instantiate<GameObject>(demoPrefab);
        tGO.transform.position = pos;
        tGO.transform.localScale = scale;
        tGO.GetComponent<Renderer>().material.color = Color.blue;

        // Create anchor to house rooms
        GameObject anchor;
        anchor = GameObject.Find("anchorMap");
        if (anchor == null) anchor = new GameObject("anchorMap");
        tGO.transform.SetParent(anchor.transform);

        // Set room GameObject
        room.roomGO = tGO.gameObject;
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
}

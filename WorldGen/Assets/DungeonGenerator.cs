using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Random = System.Random;

public class DungeonGenerator : MonoBehaviour
{
    enum CellType
    {
        Empty,
        Room,
        Hall
    }

    class Room
    {
        public RectInt bounds;

        public Room(Vector2Int pos, Vector2Int size)
        {
            bounds = new RectInt(pos, size);
        }

        // TODO need to fix
        public static bool Intersect(Room a, Room b)
        {
            // return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
            //     || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
            foreach (var pos in a.bounds.allPositionsWithin)
            {
                if (b.bounds.Contains(pos)) return true;
            }
            return false;
        }
    }

    public Vector2Int dungeonSize;
    public int numRooms;
    public Vector2Int roomMaxSize;
    public Vector2Int roomMinSize;
    public GameObject roomPrefab;

    GridMap<CellType> map;
    List<Room> rooms;
    // Random random;

    void Start()
    {
        GenerateDungeon();
    }

    // TODO Create Procedurally generated dungeon
    void GenerateDungeon()
    {
        // random = new Random();
        map = new GridMap<CellType>(dungeonSize, Vector2Int.zero);
        rooms = new List<Room>();

        GenerateRooms();
    }

    // TODO Create funtion to build rooms
    void GenerateRooms()
    {
        for (int i = 0; i < numRooms; i++)
        {
            // TODO Generate random Pos and Size
            // int xPos = random.Next(0, dungeonSize.x);
            // int yPos = random.Next(0, dungeonSize.y);
            int xPos = Random.Range(0, dungeonSize.x);
            int yPos = Random.Range(0, dungeonSize.y);
            Vector2Int tempPos = new Vector2Int(xPos, yPos);

            // int xSize = random.Next(roomMinSize.x, roomMaxSize.x + 1);
            // int ySize = random.Next(roomMinSize.y, roomMaxSize.y + 1);
            int xSize = Random.Range(roomMinSize.x, roomMaxSize.x + 1);
            int ySize = Random.Range(roomMinSize.y, roomMaxSize.y + 1);
            Vector2Int tempSize = new Vector2Int(xSize, ySize);

            // TODO Generate Room
            Room tempRoom = new Room(tempPos, tempSize);
            Room roomBuffer = new Room(tempPos + new Vector2Int(-1, -1), tempSize + new Vector2Int(2, 2));

            // TODO Check if room valid
            bool valid = true;
            // if room intersects
            foreach (var r in rooms)
            {
                if (Room.Intersect(r, roomBuffer))
                {
                    valid = false;
                    break;
                }
            }
            // if room out of bounds
            if ((tempRoom.bounds.xMin < 0 || tempRoom.bounds.xMax >= dungeonSize.x)
            || (tempRoom.bounds.yMin < 0 || tempRoom.bounds.yMax >= dungeonSize.y))
            {
                valid = false;
            }
            
            if(valid)
            {
                // TODO Add room if valid
                rooms.Add(tempRoom);
                foreach (var pos in tempRoom.bounds.allPositionsWithin)
                {
                    map[pos] = CellType.Room;
                }

                // TODO Debug, build rooms to see results
                BuildRoom(tempRoom.bounds.position, tempRoom.bounds.size);
            }
        }
    }

    void BuildRoom(Vector2Int pos, Vector2Int size)
    {
        Vector3 tempPos = new Vector3(pos.x, 0, pos.y);
        Vector3 tempScale = new Vector3(size.x, 1, size.y);

        GameObject temp = Instantiate<GameObject>(roomPrefab, tempPos, Quaternion.identity);
        temp.transform.localScale = tempScale;
        temp.GetComponent<Renderer>().material.color = Color.blue;

        GameObject anchor = GameObject.Find("roomAnchor");
        if (anchor != null) temp.transform.SetParent(anchor.transform);
    }

    // TODO FindPathways
    void FindHallways()
    {

    }

    // TODO BuildHallways
    void BuildHallways()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum Cell {
        Empty,
        Room,
        Hall
    }

    class Room {
        public RectInt bounds;

        public Room(Vector2Int pos, Vector2Int size){
            this.bounds = new RectInt(pos, size);
        }

        public Room(RectInt bounds){
            this.bounds = bounds;
        }

        public bool Intersect(Room r1, Room r2){
            return r1.bounds.Overlaps(r2.bounds);
        }

        public Vector2Int size;
        public int numRooms;
        public Vector2Int roomMaxSize;

        GridMap<Cell> map;
        List<Room> rooms;

        void Start() {
            BuildDungeon();
        }

        // TODO Create Procedurally generated dungeon
        void BuildDungeon(){
            
        }

        // TODO Create funtion to build rooms
        void BuildRooms(){

        }

        // TODO FindPathways
        void FindHallways(){

        }

        // TODO BuildHallways
        void BuildHallways(){
            
        }
    }      
}

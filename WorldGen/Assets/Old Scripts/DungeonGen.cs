using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType{
    Empty,
    Wall,
    Hall,
    None
}

public class Dungeon{
    private RoomType[] rooms;
    private int width=0;
    private int length=0;

    public Dungeon(int length, int width){
        this.width = width;
        this.length = length;
        rooms = new RoomType[width*length];
        EmptyRooms();
    }

    public int Width{
        get{ return this.width;}
        //set{ this.width = value;}
    }

    public int Length{
        get{ return this.length;}
        //set{ this.length = value;}
    }

    public RoomType GetRoom(int row, int col){
        if(!isValid(row, col)) return RoomType.None;
        int index = row*width + col;
        RoomType room = rooms[index];
        return room;
    }

    public bool SetRoom(RoomType room, int row, int col){
        if(!isValid(row, col)) return false;
        int index = row*width + col;
        rooms[index] = room;
        return true;
    }

    public void EmptyRooms(){
        for(int row=0; row<length; row++){
            for(int col=0; col<width; col++){
                SetRoom(RoomType.Empty, row, col);
            }
        }
    }

    public bool GenerateRoom(int row, int col, int length, int width){
        // check if in bounds
        if(!isValid(row+length, col+width)) return false;

        // check if rooms are empty
        for(int r=row; r<length;r++){
            for(int c=col; c<width; c++){
                if(!isEmpty(r,c)) return false;
            }
        }
        
        // if empty, create area
        for(int r=row; r<length;r++){
            for(int c=col; c<width; c++){
                SetRoom(RoomType.Wall, r, c);
            }
        }
        
        return true;
    }

    // Checks if given coords are contain empty room
    public bool isEmpty(int row, int col){
        if(!isValid(row, col)) return false;
        return (GetRoom(row, col).Equals(RoomType.Empty));
    }

    // Checks if given coords are in the dungeon
    public bool isValid(int row, int col){
        bool rowValid = (row>=0 && row < length);
        bool colValid = (col>=0 && col < width);
        return rowValid && colValid;
    }

    public void DemoDungeon(){
        GameObject parent = new GameObject("demoAnchor");
        for(int r=0; r<length; r++){
            for(int c=0; c<width; c++){
                GameObject tempGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tempGO.transform.position = new Vector3(c-width/2,-r+length/2, 0);
                Color tempC = Color.red;
                string name = "";
                RoomType type = GetRoom(r, c);
                switch(type){
                    case RoomType.Empty:
                        tempC = Color.black;
                        name = "EmptyRoom";
                        break;

                    case RoomType.Wall:
                        tempC = Color.blue;
                        name = "Wall";
                        break;
                    
                    case RoomType.Hall:
                        tempC = Color.white;
                        name = "Hall";
                        break;
                }
                tempGO.GetComponent<Renderer>().material.color = tempC;
                tempGO.name = name;
                tempGO.transform.SetParent(parent.transform);
            }
        }
    }

    public override string ToString(){
        string tempStr = "";
        for(int row=0; row<length; row++){
            for(int col=0; col<width; col++){
                RoomType type = GetRoom(row, col);
                switch(type){
                    case RoomType.Empty:
                        tempStr += "[ ] ";
                        break;

                    case RoomType.Hall:
                        tempStr += "[/] ";
                        break;
                    
                    case RoomType.Wall:
                        tempStr += "[#] ";
                        break;
                    
                    default:
                        break;
                }
            }
            tempStr = tempStr + "\n";
        }
        return tempStr;
    }
}

public class DungeonGen : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int dungeonLength;
    public int dungeonWidth;
    public int dungeonHeight;

    public int roomMinLength;
    public int roomMaxLength;
    public int roomMinWidth;
    public int roomMaxWidth;
    public int roomMaxCount;
    
    //* Might make singleon?
    private Dungeon dungeon;

    private void Start()
    {
        // Create dungeon for generation
        dungeon = new Dungeon(dungeonLength, dungeonWidth);
        //// Test to see if working
        //// dungeon.SetRoom(RoomType.Wall, 0,0);
        //// dungeon.SetRoom(RoomType.Hall, 0,1);
        //// string str = dungeon.ToString();
        //// print(str);

        GenerateRooms();
        // This works so something wrong with generation
        //dungeon.SetRoom(RoomType.Wall, 0, 0);
        dungeon.DemoDungeon();
    }

    private void GenerateRooms(){
        for(int attempt=0; attempt<roomMaxCount; attempt++){
            int width = Random.Range(roomMinWidth, roomMaxWidth);
            int length = Random.Range(roomMinLength, roomMaxLength);
            int row = Random.Range(0,dungeon.Length-roomMinLength);
            int col = Random.Range(0,dungeon.Width-roomMinWidth);
            dungeon.GenerateRoom(row,col,length,width);
        }           
    }
}

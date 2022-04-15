using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private string side;
    private GameObject newRoom;
    private GameObject currentRoom;
    private Room roomInfo;
    private GameObject roomsGO;
    private Rooms rooms;

    void Awake(){
        // get Rooms script from Rooms gameobject
        roomsGO = GameObject.Find("Rooms");
        rooms = roomsGO.GetComponent<Rooms>();
        side = gameObject.name.Substring(gameObject.name.Length-1);
        currentRoom = transform.parent.parent.gameObject;
        roomInfo = currentRoom.GetComponent<Room>();
        // Randomness to minimize conflicts
        // better fixes but i'm lazy af
        Invoke("Spawn", Random.Range(.1f,.2f));
    }

    void Spawn() {
        // Using the side generate room from Rooms
        int randIndex;
        switch (side)
        {
            case "N":
                //print("north spawner");
                randIndex = Random.Range(0, rooms.south.Length);
                newRoom = Instantiate<GameObject>(rooms.south[randIndex], transform.position, rooms.south[randIndex].transform.rotation);
                newRoom.transform.SetParent(roomsGO.transform, true);
                break;
            
            case "E":
                //print("east spawner");
                randIndex = Random.Range(0, rooms.west.Length);
                newRoom = Instantiate<GameObject>(rooms.west[randIndex], transform.position, rooms.west[randIndex].transform.rotation);
                newRoom.transform.SetParent(roomsGO.transform, true);
                break;
            
            case "S":
                //print("south spawner");
                randIndex = Random.Range(0, rooms.north.Length);
                newRoom = Instantiate<GameObject>(rooms.north[randIndex], transform.position, rooms.north[randIndex].transform.rotation);
                newRoom.transform.SetParent(roomsGO.transform, true);
                break;

            case "W":
                //print("west spawner");
                randIndex = Random.Range(0, rooms.east.Length);
                newRoom = Instantiate<GameObject>(rooms.east[randIndex], transform.position, rooms.east[randIndex].transform.rotation);
                newRoom.transform.SetParent(roomsGO.transform, true);
                break;
        }
        Room newRoomInfo = newRoom.GetComponent<Room>();
        if(newRoomInfo != null){
            newRoomInfo.SetNeighbor(currentRoom, side);
        }
        if(roomInfo != null){
            roomInfo.spawnedNeighbor = true;
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Room")){
            Destroy(gameObject);
        }
    }
}

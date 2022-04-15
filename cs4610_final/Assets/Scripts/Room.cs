using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject neighbor;
    public bool spawnedNeighbor = false;
    public string neighborSide = "";

    private Transform spawnPoints;
    private GameObject roomsGO;
    private Rooms rooms;
    
    void Start(){
        for(int i=0; i<transform.childCount; i++){
            Transform temp = transform.GetChild(i);
            if(temp.name == "SpawnPoints"){
                spawnPoints = temp;
            } // if not found, fucking panic
        }
    }

    void FixedUpdate(){
        if(spawnPoints.childCount < 1 && spawnedNeighbor == false){
            print("Found conflict, fixing...");
            GameObject newRoom;
            roomsGO = GameObject.Find("Rooms");
            rooms = roomsGO.GetComponent<Rooms>();

            switch (neighborSide)
            {
                case "N":
                    //print("north spawner");
                    newRoom = Instantiate<GameObject>(rooms.south[0], transform.position, rooms.south[0].transform.rotation);
                    newRoom.transform.SetParent(roomsGO.transform, true);
                    break;
                
                case "E":
                    //print("east spawner");
                    newRoom = Instantiate<GameObject>(rooms.west[0], transform.position, rooms.west[0].transform.rotation);
                    newRoom.transform.SetParent(roomsGO.transform, true);
                    break;
                
                case "S":
                    //print("south spawner");
                    newRoom = Instantiate<GameObject>(rooms.north[0], transform.position, rooms.north[0].transform.rotation);
                    newRoom.transform.SetParent(roomsGO.transform, true);
                    break;

                case "W":
                    //print("west spawner");
                    newRoom = Instantiate<GameObject>(rooms.east[0], transform.position, rooms.east[0].transform.rotation);
                    newRoom.transform.SetParent(roomsGO.transform, true);
                    break;
            }
            Destroy(gameObject);
        }
    }
    
    public void SetNeighbor(GameObject neighbor, string neighborSide){
        this.neighbor = neighbor;
        this.neighborSide = neighborSide;
    }
}

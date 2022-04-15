using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour
{
    [Header("Set in Inspector:  If there is a room in scene leave none")]
    public GameObject startingRoom;
    
    [Header("Set in Inspector:  'Dead End' Rooms must be in ele 0")]
    public GameObject[] north;
    public GameObject[] east;
    public GameObject[] south;
    public GameObject[] west;

    private bool setup = false;

    public List<GameObject> roomsList;

    void Start(){
        print("Generating Rooms...");
        if(startingRoom != null){
            GameObject spawnRoom = Instantiate<GameObject>(startingRoom);
            spawnRoom.transform.SetParent(transform, true);
            spawnRoom.transform.position = Vector3.zero;
        }
    }

    void Update(){
        if(GameObject.FindGameObjectWithTag("SpawnPoint") == null && !setup){
            SetupRooms();
        }
    }

    // !Sometimes there will be conflict at end when there are 0 spawnpoints need to add wait timer.
    void SetupRooms(){
        setup = true;
        print("Setting up rooms...");
        for(int i=0; i<transform.childCount; i++){
            roomsList.Add(transform.GetChild(i).gameObject);
        }
        print(roomsList.Count);
        // TODO Add boss room generation
        
        // TODO ADD chest room

        // TODO add random enemy rooms
    }
}

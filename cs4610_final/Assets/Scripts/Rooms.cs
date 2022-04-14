using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour
{
    public GameObject startingRoom;
    public GameObject[] top;
    public GameObject[] bottom;
    public GameObject[] left;
    public GameObject[] right;

    private GameObject spawnRoom;

    void Start(){
        spawnRoom = Instantiate<GameObject>(startingRoom);
        spawnRoom.transform.SetParent(transform, true);
        spawnRoom.transform.position = Vector3.zero;
    }
}

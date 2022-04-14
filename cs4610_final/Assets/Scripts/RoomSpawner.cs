using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public bool spawned = false;

    void Start(){
        Invoke("Spawn", 1f);
    }

    void Spawn() {
        // Find what side the spawner is on
        string side = gameObject.name.Substring(gameObject.name.Length-1);

        // get Rooms script from Rooms gameobject
        GameObject roomsGO = GameObject.Find("Rooms");
        Rooms rooms = roomsGO.GetComponent<Rooms>();

        // Using the side generate room from Rooms
        int randIndex;
        GameObject temp;
        switch (side)
        {
            case "T":
                //print("top spawner");
                randIndex = Random.Range(0, rooms.bottom.Length);
                temp = Instantiate<GameObject>(rooms.bottom[randIndex], transform.position, rooms.bottom[randIndex].transform.rotation);
                temp.transform.SetParent(roomsGO.transform, true);
                break;
            
            case "R":
                //print("right spawner");
                randIndex = Random.Range(0, rooms.left.Length);
                temp = Instantiate<GameObject>(rooms.left[randIndex], transform.position, rooms.left[randIndex].transform.rotation);
                temp.transform.SetParent(roomsGO.transform, true);
                break;
            
            case "B":
                //print("bottom spawner");
                randIndex = Random.Range(0, rooms.top.Length);
                temp = Instantiate<GameObject>(rooms.top[randIndex], transform.position, rooms.top[randIndex].transform.rotation);
                temp.transform.SetParent(roomsGO.transform, true);
                break;

            case "L":
                //print("left spawner");
                randIndex = Random.Range(0, rooms.right.Length);
                temp = Instantiate<GameObject>(rooms.right[randIndex], transform.position, rooms.right[randIndex].transform.rotation);
                temp.transform.SetParent(roomsGO.transform, true);
                break;

            default:
                break;
        } 
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Room")){
            Destroy(gameObject);
        }
    }
}

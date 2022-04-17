using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    void OnCollisionEnter(Collision other){
        if(gameObject.tag == "Room") return;
        if(other.gameObject.tag == "Room"){
            Destroy(gameObject);
        }
    }
}

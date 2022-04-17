using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other){
        if(other.transform.tag == "Room"){
            Destroy(this.gameObject);
        }
    }
}

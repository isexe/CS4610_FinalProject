using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public List<NavMeshSurface> surfaces;

    void Start(){
        surfaces = new List<NavMeshSurface>();
        Invoke("BakeMesh", .5f);
    }

    // Use this for initialization
    void BakeMesh() 
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("NavMesh");

        foreach(var room in rooms){
            surfaces.Add(room.GetComponent<NavMeshSurface>());
        }

        for (int i = 0; i < surfaces.Count; i++) 
        {
            surfaces [i].BuildNavMesh ();    
        }    
    }
}

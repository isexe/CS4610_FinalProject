using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dog_enemy_control : MonoBehaviour
{
    GameObject player;
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    float disFromPlayer;
    int health;
    // Start is called before the first frame update
    void Start()
    {
        health = 20;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.Find("FPSControl 1");
        Playerpos = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        disFromPlayer = Vector3.Distance (player.transform.position, gameObject.transform.position);
        if(health == 0)
        {
            Destroy(gameObject);
        }
        agent.destination = Playerpos.position;
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Bullet")
        {
            print("collision");
            health -= 10;   
        }
    }
}

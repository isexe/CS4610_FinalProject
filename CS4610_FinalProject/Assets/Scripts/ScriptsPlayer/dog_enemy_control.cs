using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dog_enemy_control : MonoBehaviour
{
    public int health = 20;
    GameObject player;
    public Transform Playerpos;
    UnityEngine.AI.NavMeshAgent agent;
    float disFromPlayer;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) Playerpos = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            GameObject.FindGameObjectWithTag("Player");
            if(player != null) Playerpos = player.GetComponent<Transform>();
        }
        else{
            disFromPlayer = Vector3.Distance (player.transform.position, gameObject.transform.position);
            if(health == 0)
            {
                Destroy(this.gameObject);
            }
            if(disFromPlayer < 10){
                agent.destination = Playerpos.position;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Bullet")
        {
            print("collision");
            health -= 10;
            if(collision.name != "scimitar") Destroy(collision.gameObject);
        }
    }
}

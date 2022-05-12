using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class player_controller : MonoBehaviour
{
    //Weapons
    GameObject rifle;
    GameObject sword;
    //Player Stats
    int health;
    //UI
    TMP_Text healthText;
    // Start is called before the first frame update
    void Start()
    {
        rifle = GameObject.Find("rifle");
        sword = GameObject.Find("scimitar");

        if(sword != null) sword.SetActive(false);

        health = 100;

        healthText = GameObject.Find("healthnum").GetComponent<TMP_Text>();
        healthText.text = "Health: 100";

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            sword.SetActive(true);
            rifle.SetActive(false);
        }
        if (Input.GetKeyDown("2"))
        {
            rifle.SetActive(true);
            sword.SetActive(false);

        }

        if(health <= 0){
            SceneManager.LoadScene(0);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Zombie")
        {
            print("collision");
            health -= 10; 
            healthText.text = "Health: " + health.ToString();  
        }
    }

}

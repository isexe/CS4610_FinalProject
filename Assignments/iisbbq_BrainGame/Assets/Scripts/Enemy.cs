using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int maxNumber = 15;
    public int minNumber = 1;
    public float speed = 1f;

    public int damageF = 3;
    public int damageD = 2;
    public int damageS = 1;
    
    [Header("Set Dynamically")]
    public TextMeshProUGUI numberText;
    public int health;

    public void SetupEnemy(int minNum, int maxNum, float fallSpeed)
    {
        maxNumber = maxNum;
        minNumber = minNum;
        speed = fallSpeed;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (numberText == null)
        {
            numberText = transform.GetComponentInChildren<TextMeshProUGUI>();
        }

        health = Random.Range(minNumber, maxNumber + 1);
        numberText.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        MoveDown();
        UpdateHealth();
    }

    void MoveDown()
    {
        Vector3 tempPos = transform.position;
        tempPos.y -= speed * Time.deltaTime;
        transform.position = tempPos;
    }

    void UpdateHealth()
    {
        if(health == 0)
        {
            Destroy(gameObject);
        }
        numberText.text = health.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if(other.gameObject.tag == "Player")
        {
            // Need to add health system here
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "ProjectileF")
        {
            health -= damageF;
            if(health < 0)
            {
                health += damageF;
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "ProjectileD")
        {
            health -= damageD;
            if (health < 0)
            {
                health += damageD;
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "ProjectileS")
        {
            health -= damageS;
            if (health < 0)
            {
                health += damageS;
            }
            Destroy(other.gameObject);
        }
    }
}

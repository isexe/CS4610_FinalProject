using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    // Add game restart delay in public set in header section
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    // Replace public float shieldLevel with private float _shieldLevel = 1;
    [SerializeField]
    private float _shieldLevel = 1;

    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGO = null;

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4); // Sets shield to value less than or equal to 4
            if(value < 0)                                   // If shield is less than 0
            {
                Destroy(this.gameObject);                   // Destroys object
                Main.S.DelayedRestart(gameRestartDelay);    // and restarts the game
            }
        }
    }

    private void Update()
    {
        //Add to the end of update
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }
    }

    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        rigidB.velocity = Vector3.up * projectileSpeed;
    }

    // From page 233
    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // Make sure it's a new object
        if (go == lastTriggerGO)
        {
            return;
        }
        lastTriggerGO = go;

        if (go.tag == "Enemy")  // If the shield was triggered by an enemy
        {
            shieldLevel--;      // Decrement the shield level
            Destroy(go);        // and destroy the enemy
        } 
        else
        {
            print("Trigggered by non-Enemy: " + go.name);
        }
    }
}

public class Main : MonoBehaviour
{
    // insert after public void spawnEnemy()
    public void DelayedRestart(float delay)
    {
        // Invoke the restart() method in delayed seconds
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // Reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }
}

public class Enemy : MonoBehaviour
{
    // insert under virtual void move
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        if(otherGO.tag == "ProjectileHero")
        {
            Destroy(otherGO);
            Destroy(gameObject);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
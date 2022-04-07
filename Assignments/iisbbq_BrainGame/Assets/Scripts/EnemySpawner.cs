using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float maxDistX = 50f;
    public float minDistX = -50f;
    public float distY = 40f;
    public float distZ = 0f;
    public float spawnSpeed = 3f;
    public int maxSpawn = 10;
    public GameObject enemyPrefab = null;

    private List<GameObject> enemyGO;

    // Start is called before the first frame update
    void Start()
    {
        if(enemyPrefab == null)
        {
            return;
        }
        enemyGO = new List<GameObject>();
        Invoke("spawnEnemy", spawnSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnEnemy()
    {
        if(enemyGO.Count < maxSpawn)
        {
            Vector3 tempPos = new Vector3(Random.Range(minDistX, maxDistX), distY, distZ);
            GameObject temp = Instantiate<GameObject>(enemyPrefab);
            temp.transform.position = tempPos;
            enemyGO.Add(temp); 
        }
        Invoke("spawnEnemy", spawnSpeed);
    }
}

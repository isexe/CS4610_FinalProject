using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int totalSpawned;
    public Vector2 maxDistance;
    public Vector2 minDistance;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<totalSpawned; i++){
            float x = Random.Range(minDistance.x, maxDistance.x);
            float z = Random.Range(minDistance.y, maxDistance.y);

            Vector3 pos = new Vector3(this.transform.position.x + x, 2.5f, this.transform.position.z + z);

            GameObject temp = Instantiate<GameObject>(prefab, pos, gameObject.transform.rotation);
            temp.transform.SetParent(this.transform);
            temp.name = "Boss";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Boss") == null){
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}

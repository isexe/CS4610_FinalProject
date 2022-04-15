using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missilefollow : MonoBehaviour
{
    public GameObject enemyTracked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, enemyTracked.transform.position, 0.01f);
        transform.position = Vector3.MoveTowards(transform.position, enemyTracked.transform.position, Time.deltaTime * 10.0f);
        transform.LookAt(enemyTracked.transform);
    }
}

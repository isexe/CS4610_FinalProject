using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Test : MonoBehaviour
{
    public Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        pos = Random.insideUnitSphere * 5;

        Vector3 vel = Random.insideUnitSphere * 2;
        rigid.velocity = vel;

        Color randColor = new Color(Random.value, Random.value, Random.value);

        Renderer rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", randColor);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 pos
    {
        get { return transform.position; }
        set { transform.position = value; }
    }


}

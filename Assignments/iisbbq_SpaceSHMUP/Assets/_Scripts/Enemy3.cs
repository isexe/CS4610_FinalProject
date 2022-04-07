using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : Enemy
{
    [Header("Set in Inspector:  Enemy_3")]
    public float lifeTime = 5f;

    [Header("Set Dynamically:  Enemy_3")]
    public Vector3[] points;
    public float birthTime;

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[3];

        // The starting pos has been set in Main.SpawnEnemy()
        points[0] = pos;

        // set xMin and xMax same way as Main.SpawnEnemy()
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        
        // pick a random middle posiiton at bottom 
        Vector3 v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        // pick a random final position at top
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        // set the birthTime
        birthTime = Time.time;
    }

    public override void Move()
    {
        // Bezier curve u value
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            // This enemy has lived it's life
            Destroy(this.gameObject);
            return;
        }

        // interpolate the three brazier curve points
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}

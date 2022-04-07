using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZig : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        tempPos.x -= Mathf.Sin(Time.time * Mathf.PI)*.1f;
        pos = tempPos;

        base.Move();
    }
}

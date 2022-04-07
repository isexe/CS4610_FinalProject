using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    [Header("Set in Inspector:  Enemy1")]
    public float waveFrequency = 2;
    public float waveWidth = 4;
    public float waveRotY = 45;
    
    private float x0;   // The initial x value of pos
    private float birthTime;

    // Start works well because it's not used bythe Enemy superclass
    void Start() {
        x0 = pos.x;
        birthTime = Time.time;
    }

    // Override the Move function on Enemy
    public override void Move()
    {
        // Because pos is a property, you can't directly set pos.x
        //  so get the pos as an editable Vector3
        Vector3 tempPos = pos;
        // theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + sin * waveFrequency;
        pos = tempPos;

        // rotate a bit about y
        Vector3 rot = new Vector3(0, sin*waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() still hands the movement down in y
        base.Move();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public Rigidbody rb;

    private float time = 0.0f;
    private bool isMoving = false;
    private bool isJumpPressed = false;


    void Start()
    {
        rb = GetComponent < Rigidbody > ();
    }

    void Update()
    {
        isJumpPressed = Input.GetButtonDown("Jump");
    }

void FixedUpdate()
    {
    if (isJumpPressed)
    {
        // the cube is going to move upwards in 10 units per second
        rb.velocity = new Vector3(0, 10, 0);
        isMoving = true;
        Debug.Log("jump");
    }

    if (isMoving)
    {
        // when the cube has moved for 10 seconds, report its position
        time = time + Time.fixedDeltaTime;
        if (time > 10.0f)
        {
            Debug.Log((gameObject.transform.position.y + " : " + time));
            time = 0.0f;
        }
    }
}
}

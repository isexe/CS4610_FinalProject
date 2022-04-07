using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTreeExtreme: MonoBehaviour
{
    [Header("Set in Inspector")]
    //Prefab for instantiating apples
    public GameObject applePrefab;

    //Apple Tree movement speed
    public float speed = 1.5f;

    public float maxSpeed = 50f;

    //Distance where apple tree turns around
    public float leftAndRightEdge = 10f;

    //Chance that the Apple Tree will change directions
    public float chanceToChangeDirections = 0.1f;

    public float maxChance = 0.1f;

    //Apple spawn rate
    public float secondsBetweenAppleDrops = 1f;

    public float maxSpawnRate = 1f;

    //Extreme
    public float incrementSpeed = 0f;
    public float incrementChanceForChangeDirection = 0f;
    public float decrementSecondsBetweenApples = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Dropping apples every second
        Invoke("DropApple", 2f);
    }

    void DropApple()
    {
        GameObject apple = Instantiate<GameObject>(applePrefab);
        Vector3 pos = transform.position;
        pos.z -= 1f;
        apple.transform.position = pos;
        Invoke("DropApple", secondsBetweenAppleDrops);
    }

    // Update is called once per frame
    void Update()
    {
        if(speed < maxSpeed)
        {
            if (speed > 0)
            {
                speed += incrementSpeed * Time.deltaTime;
            }
            else
            {
                speed -= incrementSpeed * Time.deltaTime;
            }
        }
        if(secondsBetweenAppleDrops > maxSpawnRate)
        {
            secondsBetweenAppleDrops -= decrementSecondsBetweenApples * Time.deltaTime;
        }
        if(chanceToChangeDirections < maxChance)
        {
            chanceToChangeDirections += incrementChanceForChangeDirection * Time.deltaTime;
        }
        

        //Basic movement
        //The code below takes the position and adjusts it's x via deltaTime
        //This means the movement is time based rather than by frame keeping gameplay consistent
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;
        
        //Changing direction
        if (pos.x > leftAndRightEdge)
        {
            speed = -Mathf.Abs(speed); //Move Left
        }
        else if (pos.x < -leftAndRightEdge)
        {
            speed = Mathf.Abs(speed);  //Move Right
        }
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        //If random value [0,1] is less than chance, change directions
        //Moved to FixedUpdate() to keep the movement time-based
        if (Random.value < chanceToChangeDirections)
        {
            speed *= -1;  //Change direction
        }
    }
}

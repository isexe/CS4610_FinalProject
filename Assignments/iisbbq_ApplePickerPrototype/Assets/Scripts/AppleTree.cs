using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppleTree : MonoBehaviour
{
    [Header("Set in Inspector")]
    //Prefab for instantiating apples
    public GameObject applePrefab;
    
    //Distance where apple tree turns around
    public float leftAndRightEdge;
    
    //Apple Tree movement speed
    public float[] speedList;
    private float speed;

    

    //Chance that the Apple Tree will change directions
    public float[] chanceToChangeDirectionsList;
    private float chanceToChangeDirections;

    //Apple spawn rate
    public float[] secondsBetweenAppleDropsList;
    private float secondsBetweenAppleDrops;


    public int numberOfLevels = 5;
    private int currentLevel = 0;

    public float secondsBetweenLevels = 30.0f;

    private float timer;

    [Header("Set Dynamically")]
    public Text titleGT;


    private void Awake()
    {
        //Finds the GameObject 'Title' in the hierarchy
        GameObject titleGO = GameObject.Find("LevelTitle");
        if(titleGO != null)
        {
            titleGT = titleGO.GetComponent<Text>();
            SetLevel();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //initializes the level timer
        timer = 0;

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

        if(currentLevel < numberOfLevels-1)
        {
            timer += Time.deltaTime;
            UpdateTitle();
            if(timer >= secondsBetweenLevels)
            {
                timer = 0;
                IncrementLevel();
            }
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

    void UpdateTitle()
    {
        if(timer > 0){
            //Information on string formatting found here:
            //  https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
            titleGT.text = "Level: " + (currentLevel+1).ToString() + '\n' + (secondsBetweenLevels-timer).ToString("0.00") + " seconds til next level"; 
        } else{
            titleGT.text = "Level: " + (currentLevel+1).ToString(); 
        }
    }

    void SetLevel()
    {
        speed = speedList[currentLevel];
        chanceToChangeDirections = chanceToChangeDirectionsList[currentLevel];
        secondsBetweenAppleDrops = secondsBetweenAppleDropsList[currentLevel];
        UpdateTitle();
    }

    void IncrementLevel()
    {
        currentLevel += 1;
        SetLevel();
    }
}

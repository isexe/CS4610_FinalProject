using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // a private Singleton

    [Header("Set in Inspector")]
    public Text uitLevel;                       //The UIText_Levels Text
    public Text uitHiScore;
    public Text uitScore;
    public Text uitShots;                       //The UIText_Shots text
    public int maxNumShots = 3;
    public Text uitButton;                      //The Text on UIButton_View
    public Vector3 castlePos;                   // The place to put castles
    public GameObject[] castles;                // An array of the castles

    [Header("Set Dynamically")]
    public int level;                           // The current level
    public int levelMax;                        // The total number of levels
    public int shotsTaken;                      // The number of shots taken
    public GameObject castle;                   // The current castle
    public GameMode mode = GameMode.idle;       // The state of the game
    public string showing = "Show Slingshot";   // FollowCam mode

    private int score = 0;

    private int numOfShots;
    private int pointsPerShots = 100;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    private void Start()
    {
        numOfShots = maxNumShots;
        
        S = this; // Define singleton

        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        // Get rid of the old castle if one exists
        if (castle != null)
        {
            Destroy(castle);
        }

        // Destroy old projectiles if they exist
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        // Instantiates the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        // Reset the camera
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void UpdateGUI()
    {
        // Show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken + " of " + numOfShots;

        uitHiScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
        uitScore.text = "Current Score: " + score;
    }

    private void Update()
    {
        UpdateGUI();

        // Check for level end
        if ((mode.Equals(GameMode.playing)) && Goal.goalMet)
        {
            // Calculate Points
            int leftOverShots = maxNumShots - shotsTaken + 1;
            int points = leftOverShots * pointsPerShots;
            int bonusPoints = 0;
            if(shotsTaken == 1)
            {
                bonusPoints = pointsPerShots * (level + 1);
                points += bonusPoints;
            }
            score += points;
            Debug.Log("Points:\t" + (points) + "\nBonus:\t" + (bonusPoints));


            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }

            // Change mode to stop checking for level end
            mode = GameMode.levelEnd;
            // Zoom out
            SwitchView("Show Both");
            // Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }

        // Check for loss
        if (shotsTaken >= numOfShots && FollowCam.POI == null)
        {
            if(score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
            score = 0;
            numOfShots = maxNumShots;
            level = 0;
            pointsPerShots = 100;

            StartLevel();
        }
    }

    void NextLevel()
    {
        level++;
        if(level == levelMax)
        {
            pointsPerShots += 100;
            level = 0;
            if (numOfShots > 1)
            {
                numOfShots -= 1;
            }
            else
            {
                pointsPerShots *= 2;
            }
        }
        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView.Equals(""))
        {
            eView = uitButton.text;
        }

        showing = eView;
        switch (showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;
            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;
            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    // Static method that allows code anywhere to incrememnt shotsTaken
    public static void ShotFired()
    {
        S.shotsTaken++;
    }
}

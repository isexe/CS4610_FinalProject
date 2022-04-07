using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  //This enables use of uGUI features

public class Basket : MonoBehaviour
{
    [Header("Set Dynamically")]
    public Text scoreGT;

    // Start is called before the first frame update
    void Start()
    {
        //Finds the GameObject 'ScoreCounter' in the hierarchy
        GameObject scoreGO = GameObject.Find("ScoreCounter");
        //Sets scoreGT to scoreGO text
        scoreGT = scoreGO.GetComponent<Text>();
        //Sets text to '0'
        scoreGT.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse position
        Vector3 mousePos2D = Input.mousePosition;

        //The Camera's z position stes how far to push the mouse into 3D
        mousePos2D.z = -Camera.main.transform.position.z;

        //Convert the point from 2D screen space into 3D world space
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //Move the x position of this Basket to the x position of the mouse
        Vector3 pos = this.transform.position;
        pos.x = mousePos3D.x;
        this.transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Finds out what hit this.gameObect
        GameObject collidedWith = collision.gameObject;

        //If the object was tagged as 'Apple' it is destroyed
        if (collidedWith.tag == "Apple")
        {
            Destroy(collidedWith);

            // Parse the textof the scoreGT into an int
            int score = int.Parse(scoreGT.text);
            //Adds points for destroyed aapples
            score += 100;
            //convert the score back to string and display it
            scoreGT.text = score.ToString();

            if (score > HighScore.score)
            {
                HighScore.score = score;
            }
        }
    }
}

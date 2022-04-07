using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    static public int score = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        // If the PlayerPrefs HighScore already exists, read it
        if (PlayerPrefs.HasKey("HighScore"))
        {
            score = PlayerPrefs.GetInt("HighScore");
        }

        //Assign the high score to HighScore
        PlayerPrefs.SetInt("HighScore", score);
    }

    // Update is called once per frame
    void Update()
    {
        setHighScore();
    }

    void setHighScore()
    {
        Text gt = this.GetComponent<Text>();
        gt.text = score.ToString();

        updateHighScore();
    }

    void updateHighScore()
    {
        //Update the PlayerPrefs HighScore if necessary
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    public void resetHighScore()
    {
        score = 0;
        PlayerPrefs.SetInt("HighScore", score);
        setHighScore();
    }

}

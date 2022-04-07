using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplePicker : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject basketPrefab;
    public int numBaskets = 3;
    public float basketBottomY = -14f;
    public float basketSpacingY = 2f;
    public List<GameObject> basketList;

    public GameObject gameOverScreen;
    public GameObject gameOverlay;

    bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        basketList = new List<GameObject>();

        for (int i=0; i<numBaskets; i++)
        {
            GameObject tBacketGO = Instantiate<GameObject>(basketPrefab);
            Vector3 pos = Vector3.zero;
            pos.y = basketBottomY + (basketSpacingY * i);
            tBacketGO.transform.position = pos;
            basketList.Add(tBacketGO);
        }
    }

    public void AppleDestroyed()
    {
        if (gameOver)
        {
            return;
        }
        //Destroy all of the falling apples
        if (!SceneManager.GetActiveScene().name.Equals("Endless"))
        {
            GameObject[] tAppleArray = GameObject.FindGameObjectsWithTag("Apple");
            foreach (GameObject tGO in tAppleArray)
            {
                Destroy(tGO);
            }
        }
        

        //Destroy one of the baskets
        //Get the index of the last basket in basketList
        int basketIndex = basketList.Count - 1;

        //Get a reference to that Basket GameObject
        GameObject tBasketGO = basketList[basketIndex];
        //Removes and destroys the referenced game object
        basketList.RemoveAt(basketIndex);
        Destroy(tBasketGO);

        if(basketList.Count == 0)
        {
            //Finds the GameObject 'ScoreCounter' in the hierarchy
            GameObject scoreGO = GameObject.Find("ScoreCounter");
            //Sets scoreGT to scoreGO text
            Text scoreGT = scoreGO.GetComponent<Text>();

            gameOverlay.SetActive(false);
            gameOverScreen.SetActive(true);
            gameOver = true;

            GameObject finalScoreGO = GameObject.Find("FinalScoreText");
            Text finalScoreTO = finalScoreGO.GetComponent<Text>();

            finalScoreTO.text = "Final Score:  " + scoreGT.text;

            GameObject celebrationGO = GameObject.Find("CelebrationText");
            Text celebrationTO = celebrationGO.GetComponent<Text>();

            if(int.Parse(scoreGT.text) >= HighScore.score)
            {
                celebrationTO.text = "Congratulations, you got a high score!";
            }
            else
            {
                celebrationTO.text = "You didn't get a high score, better luck next time.";
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

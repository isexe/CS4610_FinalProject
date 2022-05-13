using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject loadScreen;
    public float loadTime;
    public Vector3 endPos;

    private bool startGame;
    private float time;
    private Vector3 startPos;
    private GameObject mainCamera;

    void Start(){
        startGame = false;
        time = 0;
        mainCamera = Camera.main.gameObject;
        startPos = mainCamera.transform.position;
    }

    void FixedUpdate(){
        if(startGame){
            time += Time.deltaTime;

            menuUI.SetActive(false);
            loadScreen.SetActive(true);
            
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, time/loadTime);

            Color t = loadScreen.GetComponent<Image>().color;
            t.a = time/loadTime;
            loadScreen.GetComponent<Image>().color = t;
            
            if(time > loadTime){
                SceneManager.LoadScene(1);
            }
        }
    }

    public void StartGame(){
        startGame = true;
    }

    public void QuitGame(){
        Application.Quit();
        Debug.Log("Exiting the Game...");
    }
}

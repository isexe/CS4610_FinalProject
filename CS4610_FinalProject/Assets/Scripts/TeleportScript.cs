using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportScript : MonoBehaviour
{
    public GameObject loadScreen;
    public float loadTime = 2f;
    bool teleporting = false;
    float time = 0;

    void Start(){
        loadScreen = GameObject.Find("LoadingScreen");
    }

    void Update()
    {
        //if(loadScreen == null) loadScreen = GameObject.Find("LoadingScreen");
        if (teleporting)
        {
            time += Time.deltaTime;

            if (time > loadTime)
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    void OnTriggerEnter(Collider obj){
        Debug.Log("Enter Teleporter");
        if(obj.tag == "Player"){
            teleporting = true;
            //FadeScreen.S.fadingOut = true;
        }
    }
}

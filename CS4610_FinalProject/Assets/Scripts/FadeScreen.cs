using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public GameObject fadeScreen;
    public float fadeTime;

    private float time;
    private bool fadingIn;
    private bool fadingOut;

    // Start is called before the first frame update
    void Start()
    {
        if(fadeScreen == null) fadeScreen = this.gameObject;
        fadingIn = true;
        fadingOut = false;
        time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(fadingIn){
            FadeIn();
            if(time > fadeTime){
                fadeScreen.SetActive(false);
            }
        }
        if(fadingOut){

        }
    }

    void FadeIn(){
        time += Time.deltaTime;

        fadeScreen.SetActive(true);

        Color t = fadeScreen.GetComponent<Image>().color;
        t.a = 1-time/fadeTime;
        fadeScreen.GetComponent<Image>().color = t;
    }
}

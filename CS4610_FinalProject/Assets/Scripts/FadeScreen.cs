using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen S;
    public GameObject fadeScreen;
    public float fadeTime;

    private float time;
    public bool fadingIn;
    public bool fadingOut;

    void Awake(){
        if(S == null){
            S = this;
        }
    }

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
                fadingIn = false;
                time = 0;
            }
        }
        if(fadingOut){
            FadeOut();
            if(time > fadeTime){
                fadeScreen.SetActive(false);
                fadingOut = false;
                time = 0;
            }
        }
    }

    void FadeIn(){
        time += Time.deltaTime;

        fadeScreen.SetActive(true);

        Color t = fadeScreen.GetComponent<Image>().color;
        t.a = 1-time/fadeTime;
        fadeScreen.GetComponent<Image>().color = t;
    }

    void FadeOut(){
        time += Time.deltaTime;

        fadeScreen.SetActive(true);

        Color t = fadeScreen.GetComponent<Image>().color;
        t.a = time/fadeTime;
        fadeScreen.GetComponent<Image>().color = t;
    }

}

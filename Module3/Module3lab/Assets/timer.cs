using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    Text clockText;
    public float countDownTimer;
    float secondsElapsed;

    // Start is called before the first frame update
    void Start()
    {
        secondsElapsed = 0;
        clockText = GetComponent<Text>();
        clockText.text = "Hello!";
    }

    // Update is called once per frame
    void Update()
    {
        if (secondsElapsed < countDownTimer)
        {
            secondsElapsed += Time.deltaTime;
            clockText.text = "Countdown Seconds Remaining = " + Math.Ceiling(countDownTimer-secondsElapsed).ToString();
            clockText.color = new Color(clockText.color.r, clockText.color.g, clockText.color.b, (countDownTimer - secondsElapsed) / countDownTimer);
        }
        else
        {
            secondsElapsed = 0;
        }

    }
}

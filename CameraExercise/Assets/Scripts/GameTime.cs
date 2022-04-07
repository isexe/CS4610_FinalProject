using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    private Text _textUITime;
    private float _startTime;

    private void Awake()
    {
        _textUITime = GetComponent<Text>();
        _startTime = Time.time;
    }

    private void Update()
    {
        float elaspedSeconds = (Time.time - _startTime);
        string timeMessage = "Elapsed time = " + elaspedSeconds.ToString("F");
        _textUITime.text = timeMessage;
    }
}

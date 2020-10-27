using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{

    public Text timerText;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time - startTime;

        //string hours = 
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2");

        //Add extra 0s
        if (t / 60 < 10)
        {
            minutes = "0" + minutes;
        }

        if (t % 60 < 10)
        {
            seconds = "0" + seconds;
        }

        timerText.text = minutes + ":" + seconds;
    }
}

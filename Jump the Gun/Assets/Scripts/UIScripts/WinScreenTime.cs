using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenTime : MonoBehaviour
{
    public Text timeText;
    // Start is called before the first frame update
    void Start()
    {
        float t = GameStats.Instance.previousTime;

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

        timeText.text = "Time: " + minutes + ":" + seconds;
    }
}

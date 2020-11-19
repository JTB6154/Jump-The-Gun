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
        float time = GameStats.Instance.finishTime;
        float minutes = Mathf.Floor(time / 60f);
        float seconds = time % 60f;
        timeText.text = "Total Time: " + minutes + ":" + seconds.ToString("n4");
    }
}

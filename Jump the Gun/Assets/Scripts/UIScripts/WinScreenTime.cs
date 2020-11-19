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
        timeText.text = "Total Time: " + GameStats.Instance.finishTime.ToString();
    }
}

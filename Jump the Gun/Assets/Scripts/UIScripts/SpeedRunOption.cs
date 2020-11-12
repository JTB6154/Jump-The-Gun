using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedRunOption : MonoBehaviour
{
    GameObject speedRunToggle;

    // Start is called before the first frame update
    void Start()
    {

        speedRunToggle = GameObject.Find("SpeedrunToggle");

        if (GameStats.Instance.speedRunning == 1)
        {
            speedRunToggle.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            speedRunToggle.GetComponent<Toggle>().isOn = false;
        }
    }

    public void toggleSpeedrun()
    {
        if (speedRunToggle.GetComponent<Toggle>().isOn)
        {
            GameStats.Instance.speedRunning = 1;
        }
        else
        {
            GameStats.Instance.speedRunning = 0;
        }
    }
}

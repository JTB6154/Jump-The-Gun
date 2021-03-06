﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public GameObject continueButton;

    // Start is called before the first frame update
    void Update()
    {
        int hasSaveData = PlayerPrefs.HasKey("hasSaveData") ? PlayerPrefs.GetInt("hasSaveData") : 0;
        int firstTime = PlayerPrefs.HasKey("firstTime") ? PlayerPrefs.GetInt("firstTime") : 1;

        if (hasSaveData == 1 && firstTime == 0)
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }
}

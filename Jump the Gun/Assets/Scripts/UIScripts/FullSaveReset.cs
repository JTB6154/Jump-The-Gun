using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSaveReset : MonoBehaviour
{
    public void ResetAllSaveData()
    {
        //Reset all save data
        GameStats.Instance.newGame = true;
        GameStats.Instance.hasSaveData = 0;
        GameStats.Instance.previousTime = 0f;
        GameStats.Instance.hasBigRecoil = 0;
        GameStats.Instance.hasRocketLauncher = 0;
        GameStats.Instance.playerPosX = -49.36f;
        GameStats.Instance.playerPosY = 74.77f;
        GameStats.Instance.playerMomentumX = 0f;
        GameStats.Instance.playerMomentumY = 0f;

        //Reset all save data
        PlayerPrefs.DeleteAll();
    }
}

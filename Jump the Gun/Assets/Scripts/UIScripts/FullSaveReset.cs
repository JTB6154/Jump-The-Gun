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
        GameStats.Instance.playerPosX = -5f;
        GameStats.Instance.playerPosY = 77f;
        GameStats.Instance.playerMomentumX = 0f;
        GameStats.Instance.playerMomentumY = 0f;

        //Reset all save data
        PlayerPrefs.SetInt("hasSaveData", 0);
        PlayerPrefs.SetFloat("previousTime", 0);
        PlayerPrefs.SetInt("bigRecoilAmmo", 0);
        PlayerPrefs.SetInt("hasBigRecoil", 0);
        PlayerPrefs.SetInt("hasRocketLauncher", 0);
        PlayerPrefs.SetInt("rocketLauncherAmmo", 0);
        PlayerPrefs.SetFloat("playerPosX", -5f);
        PlayerPrefs.SetFloat("playerPosY", 77f);
        PlayerPrefs.SetFloat("playerMomentumX", 0f);
        PlayerPrefs.SetFloat("playerMomentumY", 0f);
    }
}

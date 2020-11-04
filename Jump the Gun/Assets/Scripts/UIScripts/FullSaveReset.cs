using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSaveReset : MonoBehaviour
{
    public void ResetAllSaveData()
    {
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

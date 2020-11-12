using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : Singleton<GameStats>
{
    //Check if it has been saved
    public bool newGame = true;
    public int hasSaveData = 0;

    //Check if paused
    public bool isPaused = false;

    //Variables for saving and loading 
    public float previousTime = 0f;

    public int hasBigRecoil = 0;
    public int hasRocketLauncher = 0;

    public int bigRecoilAmmo = 2;
    public int rocketLauncherAmmo = 2;

    public float playerPosX = -5f;
    public float playerPosY = 77f;
    public float playerMomentumX = 0f;
    public float playerMomentumY = 0f;

    public bool isShotNumberGUIOn = false;

    //For changing cursor
    public float redColor = 1f;
    public float greenColor = 1f;
    public float blueColor = 1f;

     public void StartLoad()
     {
        //Check if starting a new game
        if (newGame)
        {
            hasSaveData = 0;

            ResetData();
        }
        else
        {
            hasSaveData = 1;

            //Load previous data if there is any
            previousTime = PlayerPrefs.HasKey("previousTime") ? PlayerPrefs.GetFloat("previousTime") : 0f;
            hasBigRecoil = PlayerPrefs.HasKey("hasBigRecoil") ? PlayerPrefs.GetInt("hasBigRecoil") : 0;
            hasRocketLauncher = PlayerPrefs.HasKey("hasRocketLauncher") ? PlayerPrefs.GetInt("hasRocketLauncher") : 0;
            bigRecoilAmmo = PlayerPrefs.HasKey("bigRecoilAmmo") ? PlayerPrefs.GetInt("bigRecoilAmmo") : 2;
            rocketLauncherAmmo = PlayerPrefs.HasKey("rocketLauncherAmmo") ? PlayerPrefs.GetInt("rocketLauncherAmmo") : 2;
            playerPosX = PlayerPrefs.HasKey("playerPosX") ? PlayerPrefs.GetFloat("playerPosX") : -5f;
            playerPosY = PlayerPrefs.HasKey("playerPosY") ? PlayerPrefs.GetFloat("playerPosY") : 77f;
            playerMomentumX = PlayerPrefs.HasKey("playerMomentumX") ? PlayerPrefs.GetFloat("playerMomentumX") : 0f;
            playerMomentumY = PlayerPrefs.HasKey("playerMomentumY") ? PlayerPrefs.GetFloat("playerMomentumY") : 0f;
        }
     } 

    void OnDestroy()
    {
        SaveData();
    } 

    public void SaveData()
    {
        //Save game 
        PlayerPrefs.SetInt("hasSaveData", 1);
        PlayerPrefs.SetFloat("previousTime", previousTime);
        PlayerPrefs.SetInt("bigRecoilAmmo", bigRecoilAmmo);
        PlayerPrefs.SetInt("hasBigRecoil", hasBigRecoil);
        PlayerPrefs.SetInt("hasRocketLauncher", hasRocketLauncher);
        PlayerPrefs.SetInt("rocketLauncherAmmo", rocketLauncherAmmo);
        PlayerPrefs.SetFloat("playerPosX", playerPosX);
        PlayerPrefs.SetFloat("playerPosY", playerPosY);
        PlayerPrefs.SetFloat("playerMomentumX", playerMomentumX);
        PlayerPrefs.SetFloat("playerMomentumY", playerMomentumY);
    }

    public void ResetData()
    {
        newGame = false;
        hasSaveData = 0;
        previousTime = 0f;
        hasBigRecoil = 0;
        hasRocketLauncher = 0;
        playerPosX = -5f;
        playerPosY = 77f;
        playerMomentumX = 0f;
        playerMomentumY = 0f;

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

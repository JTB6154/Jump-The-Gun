using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : Singleton<GameStats>
{
    //Check if it has been saved
    public bool newGame = false;
    public int hasSaveData = 0;

    //Check if paused
    public bool isPaused = false;

    //Variables for saving and loading 
    public float gameTime = 0f;
    public int bigRecoilAmmo = 2;
    public int rocketLauncherAmmo = 2;
    public float playerPosX = 0f;
    public float playerPosY = 0f;
    public float playerMomentumX = 0f;
    public float playerMomentumY = 0f;

    void Start()
    {
        //Check if starting a new game
        if (newGame)
        {
            hasSaveData = 0;
        }
        else
        {
            hasSaveData = PlayerPrefs.HasKey("hasSaveData") ? PlayerPrefs.GetInt("hasSaveData") : 0;
        }

        if (hasSaveData == 1)
        {
            //Load previous data if there is any
            gameTime = PlayerPrefs.HasKey("previousTime") ? PlayerPrefs.GetInt("previousTime") : 0f;
            bigRecoilAmmo = PlayerPrefs.HasKey("bigRecoilAmmo") ? PlayerPrefs.GetInt("bigRecoilAmmo") : 2;
            rocketLauncherAmmo = PlayerPrefs.HasKey("rocketLauncherAmmo") ? PlayerPrefs.GetInt("rocketLauncherAmmo") : 2;
            playerPosX = PlayerPrefs.HasKey("playerPosX") ? PlayerPrefs.GetInt("playerPosX") : 0f;
            playerPosY = PlayerPrefs.HasKey("playerPosY") ? PlayerPrefs.GetInt("playerPosY") : 0f;
            playerMomentumX = PlayerPrefs.HasKey("playerMomentumX") ? PlayerPrefs.GetInt("playerMomentumX") : 0f;
            playerMomentumY = PlayerPrefs.HasKey("playerMomentumY") ? PlayerPrefs.GetInt("playerMomentumY") : 0f;
        }
    }

    void OnDestroy()
    {
        //Save game on destroy
        PlayerPrefs.SetInt("hasSaveData", 1);
        PlayerPrefs.SetFloat("previousTime", gameTime);
        PlayerPrefs.SetInt("bigRecoilAmmo", bigRecoilAmmo);
        PlayerPrefs.SetInt("rocketLauncherAmmo", rocketLauncherAmmo);
        PlayerPrefs.SetFloat("playerPosX", playerPosX);
        PlayerPrefs.SetFloat("playerPosY", playerPosY);
        PlayerPrefs.SetFloat("playerMomentumX", playerMomentumX);
        PlayerPrefs.SetFloat("playerMomentumY", playerMomentumY);
    }
}

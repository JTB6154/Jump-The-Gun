using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : Singleton<GameStats>
{
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
}

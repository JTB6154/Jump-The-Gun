using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //Load main game
    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        GameStats.Instance.isPaused = false;
        SceneManager.LoadScene("JTGFullGame", LoadSceneMode.Single);
    }

    //Load title screen
    public void LoadMenuScene()
    {
        GameStats.Instance.isPaused = false;
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    //Load Options

}

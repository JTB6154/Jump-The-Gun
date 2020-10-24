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
        SceneManager.LoadScene("JTGFullGame", LoadSceneMode.Single);
    }

    //Load title screen
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    //Load Options

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //Load main game
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GavinTest", LoadSceneMode.Single);
    }

    //Load title screen
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    //Load Options

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public GameObject continueButton;

    void Start()
    {
        int hasSaveData = PlayerPrefs.HasKey("hasSaveData") ? PlayerPrefs.GetInt("hasSaveData") : 0;

        if (hasSaveData == 1 && continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    public void LoadContinueGameScene()
    {
        GameStats.Instance.isPaused = false;
        GameStats.Instance.newGame = false;
        GameStats.Instance.StartLoad();
        Time.timeScale = 1f;
        SceneManager.LoadScene("JTGFullGame", LoadSceneMode.Single);
    }

    //Load main game
    public void LoadNewGameScene()
    {
        GameStats.Instance.isPaused = false;
        GameStats.Instance.newGame = true;
        GameStats.Instance.StartLoad();
        Time.timeScale = 1f;
        SceneManager.LoadScene("JTGFullGame", LoadSceneMode.Single);
    }

    //Load title screen
    public void LoadMenuScene()
    {
        GameStats.Instance.isPaused = false;
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
        GameStats.Instance.SaveData();
    }

    public void LoadOptionsScene()
    {
        GameStats.Instance.isPaused = false;
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Single);
    }

    //Load Options

}

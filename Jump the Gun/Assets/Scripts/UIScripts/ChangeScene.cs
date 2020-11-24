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
        if (Options.Instance.UpdatingKeyCode) return;
        GameStats.Instance.isPaused = false;
        Cursor.visible = true;
        GameStats.Instance.SaveData();
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    public void LoadMenuSceneFromWin()
    {
        if (Options.Instance.UpdatingKeyCode) return;
        GameStats.Instance.isPaused = false;
        Cursor.visible = true;
        GameStats.Instance.ResetData();
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    //Load Options
    public void LoadOptionsScene()
    {
        GameStats.Instance.isPaused = false;
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Single);
    }

    //Load Credits
    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }
    
    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        //Re-enable shooty shoot
        GameObject.Find("Player Character").GetComponent<CharacterController>().isPaused = false;

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        //Make sure player can't shoot while paused
        //Re-enable shooty shoot
        GameObject.Find("Player Character").GetComponent<CharacterController>().isPaused = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

}

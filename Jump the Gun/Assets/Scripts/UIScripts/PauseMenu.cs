using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject ammoUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStats.Instance.isPaused)
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
        ammoUI.SetActive(true);

        Time.timeScale = 1f;
        GameStats.Instance.isPaused = false;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        ammoUI.SetActive(false);

        Time.timeScale = 0f;
        GameStats.Instance.isPaused = true;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

}

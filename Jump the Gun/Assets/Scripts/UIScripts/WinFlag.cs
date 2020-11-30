using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFlag : MonoBehaviour
{

    public GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name + "" + player.gameObject.name);
        if (collision.gameObject.name == player.gameObject.name)
        {
            AudioManager.Instance.StopLoop("Ambience/Ambient1", SoundBus.BackgroundMusic);
            Cursor.visible = true;
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
            GameStats.Instance.ResetData();
        }
    }

}

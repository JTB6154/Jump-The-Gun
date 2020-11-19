using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFlag : MonoBehaviour
{

    public GameObject player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == player.gameObject.name )
        {
            GameStats.Instance.finishTime = GameStats.Instance.previousTime;
            Cursor.visible = true;
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
            GameStats.Instance.SaveData();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFlag : MonoBehaviour
{

    public GameObject player;

    void Update()
    {
        if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            Cursor.visible = true;
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
            GameStats.Instance.SaveData();
        }
    } 
    
}

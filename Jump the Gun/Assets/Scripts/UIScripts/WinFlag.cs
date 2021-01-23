using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFlag : MonoBehaviour
{
    public GameObject player;
    public SceneFader fader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name + "" + player.gameObject.name);
        if (collision.gameObject.name == player.gameObject.name)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            fader.FadeTo("WinScene");
            AudioManager.Instance.StopLoop("Ambience/Ambient1", SoundBus.BackgroundMusic);
            Cursor.visible = true;
            GameStats.Instance.ResetData();
        }
    }

}

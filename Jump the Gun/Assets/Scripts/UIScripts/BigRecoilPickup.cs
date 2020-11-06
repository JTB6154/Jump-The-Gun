using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigRecoilPickup : MonoBehaviour
{

    public GameObject player;
    public GameObject ammoUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameStats.Instance.hasBigRecoil == 1 || player.GetComponent<CharacterController>().hasBigRecoil == true)
        {
            player.GetComponent<CharacterController>().hasBigRecoil = true;
            ammoUI.SetActive(true);
            Destroy(this.gameObject);
        }
        

        if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            GameStats.Instance.hasBigRecoil = 1;
            CharacterController c = player.GetComponent<CharacterController>();
            c.hasBigRecoil = true;
            c.animator.SetTrigger("GetShotgun");
            c.animator.SetBool("Gunless", false);
            c.StartCutscene();
            ammoUI.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}

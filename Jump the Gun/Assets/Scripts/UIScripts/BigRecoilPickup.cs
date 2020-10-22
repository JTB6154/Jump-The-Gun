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
        if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            player.GetComponent<CharacterController>().hasBigRecoil = true;
            ammoUI.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}

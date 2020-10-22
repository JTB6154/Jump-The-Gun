using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJumpPickUp : MonoBehaviour
{
    public GameObject ammoUI;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            player.GetComponent<CharacterController>().hasRocketJump = true;
            ammoUI.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}

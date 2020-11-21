using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinder : MonoBehaviour
{
    [SerializeField] LayerMask defaultGround;
    [SerializeField] CharacterController player;
    Collider2D coll2D;

    private void Start()
    {
        if (player == null)
        {
            player = gameObject.GetComponentInParent<CharacterController>();
        }

        coll2D = gameObject.GetComponent<Collider2D>();
    }

    //private void Update() //depriciated code
    //{
    //    if (collider.IsTouchingLayers(defaultGround))
    //    {
    //        checkground = true;
    //    }
    //    else
    //    {
    //        checkground = false;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "reloader")
        {
            player.ReloadGuns();
        }
    }

    public bool getGrounded()
    {
        return coll2D.IsTouchingLayers(defaultGround);
    }

    public bool getGrounded(LayerMask groundMask)
    {
        return coll2D.IsTouchingLayers(groundMask);
    }






}

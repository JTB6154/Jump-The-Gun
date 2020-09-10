using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundFinder : MonoBehaviour
{
    [SerializeField] LayerMask defaultGround;
    Collider2D collider;

    private void Start()
    {
        collider = gameObject.GetComponent<Collider2D>();
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

    public bool getGrounded()
    {
        return collider.IsTouchingLayers(defaultGround);
    }

    public bool getGrounded(LayerMask groundMask)
    {
        return collider.IsTouchingLayers(groundMask);
    }

    //too busy
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("ontriggerenter");
    //    if (gameObject.GetComponent<Collider2D>().IsTouchingLayers(ground))
    //    {
    //        checkground = true;
    //    }
    //}


}

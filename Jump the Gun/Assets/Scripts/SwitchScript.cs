using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer switchSprite;
    [SerializeField] platformScript[] tiedPlatforms = {};
    [SerializeField] LayerMask tripsSwitch;
    bool switchIsOn = false;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite onSprite;


    void Start()
    {
        //initialize the switch
        GetOtherGameObjects();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger has triggered");
        if (gameObject.GetComponent<Collider2D>().IsTouchingLayers(tripsSwitch))
        {
            //if we've collided with something that trips the switch toggle it
            ToggleSwitch();
            for (int i = 0; i < tiedPlatforms.Length; i++)
            { 
                //activate the platforms here
            }
        }
    }

    void ToggleSwitch()
    {
        //toggle the switch and change the sprite
        switchIsOn = !switchIsOn;
        if (switchIsOn)
        {
            Debug.Log(switchSprite);
            Debug.Log(onSprite);
            switchSprite.sprite = onSprite;
        }
        else
        {
            Debug.Log(switchSprite);
            Debug.Log(offSprite);
            switchSprite.sprite = offSprite;
        }
    }

    void GetOtherGameObjects()
    {

        //make sure all of the things are properly attached 
        if (switchSprite = null)
        {
            Debug.LogWarning("Switch "+ gameObject.name +" automatically got sprite renderer component");
            switchSprite = GetComponent<SpriteRenderer>();
        }

        if (tiedPlatforms.Length == 0)
        {
            Debug.LogWarning("Switch " + gameObject.name + " has no platforms attached");
        }
    }

}

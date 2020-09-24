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
    [SerializeField] bool canBeTurnedOff = true;


    void Start()
    {
        //initialize the switch
        GetOtherGameObjects();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("trigger has triggered");
        if (gameObject.GetComponent<Collider2D>().IsTouchingLayers(tripsSwitch))
        {
            //if we've collided with something that trips the switch toggle it
            ToggleSwitch();

            if (!canBeTurnedOff && switchIsOn) return; //if the switch can't be turned off, and the switch is currently on, back out now
            
            foreach (platformScript platform in tiedPlatforms)
            {
                //call the platforms start transformation
                platform.toggleState();
            }
        }
    }

    private void Update()
    {
        foreach (platformScript platform in tiedPlatforms)
        {
            //call the platform that don't currently match the state of the switch
            if(platform.state != switchIsOn) platform.toggleState();
        }
    }

    void ToggleSwitch()
    {
        if (switchIsOn && !canBeTurnedOff) return; //if the switch is in the on position and it can't be turned off just back out 


        //toggle the switch and change the sprite
        switchIsOn = !switchIsOn;

        if (switchIsOn)
        {
            switchSprite.sprite = onSprite;

        }
        else
        {
            switchSprite.sprite = offSprite;
        }
    }

    void GetOtherGameObjects()
    {

        //make sure all of the things are properly attached 
        if (switchSprite == null)
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

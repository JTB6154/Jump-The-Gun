using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{

    public GameObject player;

    bool entered;
    int timesEntered;

    // Start is called before the first frame update
    void Start()
    {
        entered = false;
        timesEntered = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()) == true && entered == false)
        {
            entered = true;
            StartDialogue();
            timesEntered++;
        }
        else
        {
            //After being away from NPC for 15 seconds, they can then be triggered with next dialogue option
            Invoke("ResetEntered", 10);
        }
    }

    void ResetEntered()
    {
        entered = false;
    }

    void StartDialogue()
    {
        switch (timesEntered)
        {
            case 0:
                
                break;

            default:
                break;
        }
    }

}

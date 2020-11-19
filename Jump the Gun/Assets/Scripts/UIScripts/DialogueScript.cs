using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{

    public GameObject player;
    public GameObject dialogueTextBox;

    public List<string> dialogueOptions;

    [Range(0, 20)] [SerializeField] int textVisibleTime = 1;
    [Range(1, 20)] [SerializeField] int enterAgainTime = 10;

    bool entered;
    bool invoking;
    int timesEntered;

    // Start is called before the first frame update
    void Start()
    {
        invoking = false;
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
        }
        else if (this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()) == false && entered == true && invoking == false)
        {
            //After being away from NPC for desired seconds, they can then be triggered with next dialogue option
            invoking = true;
            Invoke("RemoveDialogue", textVisibleTime);
            Invoke("ResetEntered", enterAgainTime);
        }
    }

    void RemoveDialogue()
    {
        dialogueTextBox.SetActive(false);
    }

    void ResetEntered()
    {
        entered = false;
        invoking = false;
    }

    void StartDialogue()
    {
        //Clear dialogue box if text remains
        dialogueTextBox.GetComponent<Text>().text = "";

        //Choose dialogue
        if (timesEntered < dialogueOptions.Capacity - 1)
        {
            dialogueTextBox.GetComponent<Text>().text = dialogueOptions[timesEntered];
            //DelayText(timesEntered);
        }
        else
        {
            dialogueTextBox.GetComponent<Text>().text = dialogueOptions[dialogueOptions.Capacity - 1];
            //DelayText(dialogueOptions.Capacity - 1);
        }

        //Make dialogue visible
        dialogueTextBox.SetActive(true);

        //Adjust times entered
        timesEntered++;
    }

    void DelayText(int index)
    {
        string current = "";
        float t = Time.time;

        for (int i = 0; i < dialogueOptions[index].Length;)
        {
            if (Time.time >= t + .2f)
            {
                current += dialogueOptions[index].Substring(i, 1);
                dialogueTextBox.GetComponent<Text>().text = current;
                t = Time.time;
                i++;
            }
            else
            {
                Debug.Log(Time.time);
                Debug.Log(t);
            }
        }
    }

}

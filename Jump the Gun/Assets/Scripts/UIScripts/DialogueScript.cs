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
        //Clear current text if any
        dialogueTextBox.GetComponent<Text>().text = "";

        //Make dialogue visible
        dialogueTextBox.SetActive(true);

        //Choose dialogue
        if (timesEntered < dialogueOptions.Capacity - 1)
        {
            //dialogueTextBox.GetComponent<Text>().text = dialogueOptions[timesEntered];
            StartCoroutine(DelayText());
        }
        else
        {
            //dialogueTextBox.GetComponent<Text>().text = dialogueOptions[dialogueOptions.Capacity - 1];
            StartCoroutine(DelayRepeatText());
        }
    }

    IEnumerator DelayText()
    {

        for (int i = 1; i < dialogueOptions[timesEntered].Length + 1; i++)
        {
            dialogueTextBox.GetComponent<Text>().text = dialogueOptions[timesEntered].Substring(0, i);
            yield return new WaitForSeconds(0.08f);
        }
        //Adjust times entered
        timesEntered++;
    }

    IEnumerator DelayRepeatText()
    {

        for (int i = 1; i < dialogueOptions[dialogueOptions.Capacity - 1].Length + 1; i++)
        {
            dialogueTextBox.GetComponent<Text>().text = dialogueOptions[dialogueOptions.Capacity - 1].Substring(0, i);
            yield return new WaitForSeconds(0.08f);
        }
        //Adjust times entered
        timesEntered++;

    }

}

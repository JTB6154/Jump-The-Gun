using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : Singleton<Options>
{
    public KeyCode Left { get { return   keyHolders[0].Code; } }
    public KeyCode Right{ get { return   keyHolders[1].Code; } }
    public KeyCode Fire1 { get { return  keyHolders[2].Code; } }
    public KeyCode Fire2 { get { return  keyHolders[3].Code; } }
    public KeyCode Escape { get { return keyHolders[4].Code; } }

    private KeyHolder[] keyHolders = new KeyHolder[5];

    private bool updatingKeyCode = false;
    private int updatingKeyOfIndex;
    private Event keyEvent;

    override protected void Awake()
    {
        base.Awake();
        keyHolders = new KeyHolder[] {
            new KeyHolder("left","A"),
            new KeyHolder("right","D"),
            new KeyHolder("fire1","Mouse0"),
            new KeyHolder("fire2","Mouse1"),
            new KeyHolder("escape","Escape"),
        };

    }


    private void OnGUI()
    {
        if (updatingKeyCode)
        {
            //get what key is being pushed and assign it to the key being updated
            keyEvent = Event.current;
            if (keyEvent.isKey || keyEvent.isMouse)
            {
                updatingKeyCode = false;
                if (checkKeyCodeInUse(keyEvent))
                {
                    KeyCode temp;
                    if (keyEvent.isKey) temp = keyEvent.keyCode;
                    else temp = ConvertMouseIntToKeycode(keyEvent.button);

                    keyHolders[updatingKeyOfIndex].SetKey(temp);
                }

            }

        }
    }

    private bool checkKeyCodeInUse(Event key)
    {
        KeyCode code;
        if (keyEvent.isKey)
        {
            code = key.keyCode;
        }
        else
        {
            code = ConvertMouseIntToKeycode(key.button);
        }

        bool temp = true;
        for (int i = 0; i < keyHolders.Length; i++){
            if (keyHolders[i].Code == code) temp = false;

        }
        return temp;

    }


    public KeyCode ConvertMouseIntToKeycode(int key)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + key);
    }

    public void UpdateKeyOfIndex(int index)
    {
        updatingKeyCode = true;
        updatingKeyOfIndex = index;
    }
}

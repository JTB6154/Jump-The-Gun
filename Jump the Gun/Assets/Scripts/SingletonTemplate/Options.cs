using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : Singleton<Options>
{
    public KeyCode Left { get; set; }
    public KeyCode Right{ get; set; }
    public KeyCode Fire1 { get; set; }
    public KeyCode Fire2 { get; set; }

    private bool updatingKeyCode = false;
    private int updatingKeyOfIndex;
    private Event keyEvent;

    override protected void Awake()
    {
        base.Awake();

        Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A"));
        Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D"));
        Fire1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fire1", "Mouse0"));
        Fire2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fire2", "Mouse1"));

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
                switch (updatingKeyOfIndex) {
                    case 0:
                        if (checkKeyCodeInUse(keyEvent))
                        {
                            if(keyEvent.isKey) Left = keyEvent.keyCode;
                            if (keyEvent.isMouse) Left = ConvertMouseIntToKeycode(keyEvent.button);
                            PlayerPrefs.SetString("left", Left.ToString());
                        }
                        break;
                    case 1:
                        if (checkKeyCodeInUse(keyEvent))
                        {
                            if (keyEvent.isKey) Right = keyEvent.keyCode;
                            if (keyEvent.isMouse) Right = ConvertMouseIntToKeycode(keyEvent.button);
                            PlayerPrefs.SetString("right", Right.ToString());
                        }
                        break;
                    case 2:
                        if (checkKeyCodeInUse(keyEvent))
                        {
                            if (keyEvent.isKey) Fire1 = keyEvent.keyCode;
                            if (keyEvent.isMouse) Fire1 = ConvertMouseIntToKeycode(keyEvent.button);
                            PlayerPrefs.SetString("fire1", Fire1.ToString()); 
                        }
                        break;
                    case 3:
                        if (checkKeyCodeInUse(keyEvent))
                        {
                            if (keyEvent.isKey) Fire2 = keyEvent.keyCode;
                            if (keyEvent.isMouse) Fire2 = ConvertMouseIntToKeycode(keyEvent.button);
                            PlayerPrefs.SetString("fire2", Fire2.ToString());
                        }
                        break;
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
        return !(code == Right || code == Left || code == Fire1 || code == Fire2);

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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : Singleton<Options>
{
    public KeyCode Left { get { return   keyHolders[0].Code; } }
    public KeyCode Right{ get { return   keyHolders[1].Code; } }
    public KeyCode Fire1 { get { return  keyHolders[2].Code; } }
    public KeyCode Fire2 { get { return  keyHolders[3].Code; } }
    public KeyCode Escape { get { return keyHolders[4].Code; } }

    private KeyHolder[] keyHolders = new KeyHolder[6];

    public bool UpdatingKeyCode { get { return updatingKeyCode; } }

    private bool updatingKeyCode = false;
    bool changeUpdating = false;
    private int updatingKeyOfIndex;
    private Event keyEvent;

    public Resolution CurrentRes;
    public bool IsFullScreen;


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

        int width =  PlayerPrefs.GetInt("resWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("resHeight", Screen.currentResolution.height);
        
        CurrentRes = new Resolution();
        CurrentRes.width = width;
        CurrentRes.height = height;
        IsFullScreen = PlayerPrefs.GetInt("isFullScreen",1) == 1;
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

    private void LateUpdate()
    {
        if (changeUpdating)
        {
            changeUpdating = false;
            updatingKeyCode = false;
        }
    }

    private bool checkKeyCodeInUse(Event key)
    {
        //check if a keycode is in use
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
        //converts the mouse int from unity's Event into the keycode
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + key);
    }

    public void UpdateKeyOfIndex(int index)
    {
        //set updating KeyCode To true and updatingkeyofindex to the index of the key that is being updated.
        updatingKeyCode = true;
        updatingKeyOfIndex = index;
    }

    public void SaveResolution(Resolution res)
    {
        //write the resolution
        PlayerPrefs.SetInt("resWidth", res.width);
        PlayerPrefs.SetInt("resHeight", res.height);
    }

    public void SaveFullScreen()
    {
        //write the fullscreen value
        if (IsFullScreen) { PlayerPrefs.SetInt("isFullScreen", 1); } else { PlayerPrefs.SetInt("isFullScreen", 0); }
    }
}

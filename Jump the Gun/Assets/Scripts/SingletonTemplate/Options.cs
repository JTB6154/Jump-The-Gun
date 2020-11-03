using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : Singleton<Options>
{
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Fire1 = KeyCode.Mouse0;
    public KeyCode Fire2 = KeyCode.Mouse1;

    private bool updatingKeyCode = false;
    private int updatingKeyOfIndex;

    private void Update()
    {
        if (updatingKeyCode)
        { 
            //get what key is being pushed and assign it to the key being updated
            if(Input.
        }
    }

    public void UpdateKeyOfIndex(int index)
    {
        updatingKeyCode = true;
        updatingKeyOfIndex = index;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Camera cam;
    private SpriteRenderer rend;
    public Sprite whiteCrosshair;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        rend = GetComponent<SpriteRenderer>();

        switch (GameStats.Instance.cursorNum)
        {
            case 0:
                //Default Color
                rend.color = Color.white;
                break;

            case 1:
                rend.color = Color.black;
                break;

            case 2:
                rend.color = Color.blue;
                break;

            case 3:
                rend.color = Color.red;
                break;

            case 4:
                rend.color = Color.green;
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(cursorPos.x,cursorPos.y, transform.position.z);
    }
}

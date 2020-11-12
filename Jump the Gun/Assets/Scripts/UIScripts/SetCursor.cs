using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        rend.color = new Color(GameStats.Instance.redColor, GameStats.Instance.greenColor, GameStats.Instance.blueColor);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(cursorPos.x,cursorPos.y, transform.position.z);
    }
}

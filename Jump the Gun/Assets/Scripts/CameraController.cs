using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Camera cam = Camera.main;
    public GameObject player;

    float camHeight;
    float camWidth;

    // Start is called before the first frame update
    void Start()
    {

        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > camHeight/2)
        {
            //cam.transform = player.transform.position.y;
        }
    }
}

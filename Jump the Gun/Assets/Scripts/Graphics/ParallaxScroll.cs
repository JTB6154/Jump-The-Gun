using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public GameObject[] layers;
    private Camera mainCamera;
    private Vector2 screenBounds;
    public float choke;

    [Range(0f, 1f)]
    public float parallaxXMultiplier;

    private Vector3 lastScreenPosition;

    private float avgObjectPosZ;

    private float objectWidth;
    private float halfObjectWidth;

    void Start()
    {
        // Get player's x and y position
        var layersPosition = new Vector2(transform.parent.position.x, transform.parent.position.y);
        if (layers.Length == 0)
            Debug.LogError("Error: Please add layer to the list of layers!");
        else
        {
            float layersZ = layers[0].transform.parent.position.z;
            layers[0].transform.parent.position = new Vector3(layersPosition.x, layersPosition.y, layersZ);
        }

        // Calculate camera bounds
        mainCamera = gameObject.GetComponent<Camera>();
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        //screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        screenBounds = new Vector2(horzExtent, vertExtent);

        foreach (GameObject layer in layers)
        {
            LoadChildObjects(layer);
        }
        lastScreenPosition = transform.position;

        foreach (GameObject layer in layers)
            avgObjectPosZ += layer.transform.position.z;
        avgObjectPosZ /= layers.Length;
    }

    void LoadChildObjects(GameObject layer)
    {
        // Create left, mid, right children
        objectWidth = layer.GetComponent<SpriteRenderer>().bounds.size.x - choke;
        halfObjectWidth = objectWidth / 2.0f;

        int childrenNeeded = 2;
        GameObject clone = Instantiate(layer);
        for (int i = -1; i <= -1 + childrenNeeded; i++)
        {
            GameObject c = Instantiate(clone);
            c.transform.SetParent(layer.transform);
            c.transform.position = new Vector3(
                layer.transform.position.x + objectWidth * i, 
                layer.transform.position.y, 
                layer.transform.position.z);
            c.name = layer.name + "(" + i + ")";
        }
        Destroy(clone);
        Destroy(layer.GetComponent<SpriteRenderer>());
    }

    void RepositionChildObjects(GameObject layer)
    {
        // Get left, mid, right children
        Transform[] children = layer.GetComponentsInChildren<Transform>();
        if (children.Length > 1)
        {
            Transform leftChild = children[1];
            Transform rightChild = children[children.Length - 1];
            if (transform.position.x + screenBounds.x > rightChild.transform.position.x)
            {
                leftChild.transform.SetAsLastSibling();
                leftChild.transform.position = new Vector3(rightChild.transform.position.x + halfObjectWidth * 2, rightChild.transform.position.y, rightChild.transform.position.z);
            }
            else if (transform.position.x - screenBounds.x < leftChild.transform.position.x)
            {
                rightChild.transform.SetAsFirstSibling();
                rightChild.transform.position = new Vector3(leftChild.transform.position.x - halfObjectWidth * 2, leftChild.transform.position.y, leftChild.transform.position.z);
            }
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null) 
                continue;
            GameObject layer = layers[i];
            RepositionChildObjects(layer);

            float tempObjectPosZ = layer.transform.position.z + (layer.transform.position.z - avgObjectPosZ) * (1.0f + parallaxXMultiplier);
            float parallaxSpeedX = 1 - Mathf.Clamp01(Mathf.Abs(transform.position.z / tempObjectPosZ));
            float parallaxSpeedY = 0.5f - Mathf.Clamp01(Mathf.Abs(transform.position.z / layer.transform.position.z));
            float diffX = transform.position.x - lastScreenPosition.x;
            float diffY = transform.position.y - lastScreenPosition.y;
            layer.transform.Translate(Vector3.right * diffX * parallaxSpeedX + Vector3.up * diffY);
        }

        lastScreenPosition = transform.position;
    }

}

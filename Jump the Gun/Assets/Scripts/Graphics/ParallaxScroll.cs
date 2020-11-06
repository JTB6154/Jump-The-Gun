using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public GameObject[] levels;
    private Camera mainCamera;
    private Vector2 screenBounds;
    public float choke;

    [Range(0f, 1f)]
    public float parallaxXMultiplier;

    private Vector3 lastScreenPosition;

    private float avgObjectPosZ;

    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        foreach (GameObject obj in levels)
        {
            LoadChildObjects(obj);
        }
        lastScreenPosition = transform.position;

        foreach (GameObject obj in levels)
            avgObjectPosZ += obj.transform.position.z;
        avgObjectPosZ /= levels.Length;
    }

    void LoadChildObjects(GameObject obj)
    {
        // Create left, mid, right children
        float objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke;
        int childrenNeeded = 2;
        GameObject clone = Instantiate(obj);
        for (int i = -1; i <= -1 + childrenNeeded; i++)
        {
            GameObject c = Instantiate(clone);
            c.transform.SetParent(obj.transform);
            c.transform.position = new Vector3(objectWidth * i, obj.transform.position.y, obj.transform.position.z);
            c.name = obj.name + i;
        }
        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    void RepositionChildObjects(GameObject obj)
    {
        // Get left, mid, right children
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        if (children.Length > 1)
        {
            GameObject leftChild = children[1].gameObject;
            GameObject rightChild = children[children.Length - 1].gameObject;
            float halfObjectWidth = rightChild.GetComponent<SpriteRenderer>().bounds.extents.x - choke;
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
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject obj = levels[i];
            RepositionChildObjects(obj);
            float tempObjectPosZ = obj.transform.position.z + (obj.transform.position.z - avgObjectPosZ) * (1.0f + parallaxXMultiplier);
            float parallaxSpeedX = 1 - Mathf.Clamp01(Mathf.Abs(transform.position.z / tempObjectPosZ));
            float parallaxSpeedY = 0.5f - Mathf.Clamp01(Mathf.Abs(transform.position.z / obj.transform.position.z));
            float diffX = transform.position.x - lastScreenPosition.x;
            float diffY = transform.position.y - lastScreenPosition.y;
            obj.transform.Translate(Vector3.right * diffX * parallaxSpeedX + Vector3.up * diffY * 0.95f);
        }

        lastScreenPosition = transform.position;
    }

}

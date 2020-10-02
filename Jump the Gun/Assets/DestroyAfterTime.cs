using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeToDestruction;
    void Start()
    {
        //destroy the gameobject this script is attached to in timeToDestruction seconds
        GameObject.Destroy(gameObject, timeToDestruction);
    }


}

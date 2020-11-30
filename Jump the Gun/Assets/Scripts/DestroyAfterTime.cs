using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeToDestruction;

    [SerializeField] float fireSpeed;

    void Start()
    {
        //destroy the gameobject this script is attached to in timeToDestruction seconds
        Destroy(gameObject, timeToDestruction);
    }

    private void Update()
    {
        transform.position += transform.right * fireSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "SpriteShape")
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("Shotgun bullet hits the platform");
        Destroy(gameObject);
    }
}

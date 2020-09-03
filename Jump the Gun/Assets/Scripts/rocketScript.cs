using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [Range(1,1000)][SerializeField] float initialForce = 100f;
    float explosionforce = 0f;
    float explosionRadius = 2f;
    [SerializeField] LayerMask player;
    

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Explode();
        }
    }

    public void Init(Vector2 direction, float initExplosionForce, float initExplosionRadius)
    {
        explosionforce = initExplosionForce;
        rb.AddForce(direction.normalized * initialForce);
        explosionRadius = initExplosionRadius;
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, explosionRadius, player);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                Debug.Log("found it");
                Rigidbody2D prb = colliders[i].GetComponent<Rigidbody2D>(); //get player rigid body
                Vector3 direction = colliders[i].gameObject.transform.position - gameObject.transform.position; //vector from rocket to player
                float scale = explosionRadius / direction.magnitude; //farther away scales down force applied

                prb.AddForce(direction * scale * explosionforce);
            }
        }

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    [SerializeField] GameObject explosionParticles;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D collision;
    [SerializeField] float initialSpeed;
    [SerializeField] float randomSpeedModifier;
    private Vector2 dir;

    // Update is called once per frame
    void Update()
    {
        rb.velocity *= 1.06f;
        if (collision.IsTouchingLayers(collisionLayer))
        {
            Explode();
        }
    }

    public void Init(Vector2 direction)
    {
        initialSpeed = Random.Range(initialSpeed - randomSpeedModifier, initialSpeed);
        dir = direction;
        rb.velocity = direction.normalized * initialSpeed;
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        explosion.transform.rotation = Quaternion.Euler(0, 0, (-90 + Mathf.Atan2(-dir.y, -dir.x)) * Mathf.Rad2Deg);
        Destroy(gameObject);
    }
}

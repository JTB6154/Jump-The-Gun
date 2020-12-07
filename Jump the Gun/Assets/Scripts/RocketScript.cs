using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    [Space]
    [Header("Rocket Settings")]
    [SerializeField] Rigidbody2D rb;
    [Range(1,100)] [SerializeField] float initialSpeed = 10f;
    float explosionforce = 0f;
    float explosionRadius = 2f;
    [SerializeField] [Range(0, 100)] float MinimumForcePercent;
    [SerializeField] bool rocketVelocityZeroing = true;
    float minimumForcePercent;
    bool hasExploded = false;
    [SerializeField] LayerMask player;
    [SerializeField] public LayerMask notPlayer;
    [SerializeField] GameObject explosion_particles;

    [Space]
    [Header("Sound Settings")]
    [FMODUnity.EventRef]
    public string rocketTravelingEvent;
    [FMODUnity.EventRef]
    public string rocketExplodingEvent;


    void Start()
    {
        minimumForcePercent = MinimumForcePercent / 100f;
        //get the rigidbody if it wasn't given in the inspector
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        AudioManager.Instance.PlayOneShotAttached(rocketTravelingEvent, gameObject);
    }

    private void Update()
    {
        //check if it is colliding with the something other than the player
        if (gameObject.GetComponent<Collider2D>().IsTouchingLayers(notPlayer))
        {
            Explode();
        }
    }

    public void Init(Vector2 direction, float initExplosionForce, float initExplosionRadius)
    {
        transform.right = new Vector3(direction.x, direction.y);
        explosionforce = initExplosionForce;
        rb.velocity = direction.normalized * initialSpeed;
        explosionRadius = initExplosionRadius;
    }

    public void Explode()
    {
        if (hasExploded) return;//if the rocket has already exploded don't make it blow up again
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, explosionRadius, player);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                //Debug.Log("found it");
                Rigidbody2D prb = colliders[i].GetComponent<Rigidbody2D>(); //get player rigid body
                Vector3 direction = colliders[i].gameObject.transform.position - gameObject.transform.position; //vector from rocket to player
                float scale = (explosionRadius - direction.magnitude) / explosionRadius; //farther away scales down force applied
                direction.Normalize();
                if (rocketVelocityZeroing && scale > minimumForcePercent)
                {
                    prb.velocity = Vector2.zero;
                }
                prb.AddForce(direction * scale * explosionforce);

                if (colliders[i].GetComponent<CharacterController>().grounded)
                    Camera.main.GetComponent<CameraShake>().ShakeCamera(.15f, .3f);
            }
        }
        //show the rocket has already exploded
        hasExploded = true;
        AudioManager.Instance.PlayOneShot(rocketExplodingEvent);
        Instantiate(explosion_particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

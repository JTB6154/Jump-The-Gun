using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Character Objects")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject character;
    [SerializeField] Camera cam;
    [SerializeField] GameObject groundchecker;
    [SerializeField] LayerMask ground;
    [Space]

    [Header("Physics settings")]

    [SerializeField] float gravityScale = 1f;
    [SerializeField] float walkSpeed = 5f;
    //[SerializeField] float mass = 2.5f;
    [SerializeField] float maxSpeed = 15f;
    Vector3 moveControls = new Vector3();
    Vector3 velocity = new Vector2(0, 0);
    Vector3 smoothVelocity = Vector3.zero;
    [Range(0f, .3f)] [SerializeField] float movementSmoothing = .05f;


    public bool grounded;
    [SerializeField] float groundCheckRadius = .15f;


    [Space]

    [Header("Controls")]
    [SerializeField] KeyCode left = KeyCode.A;
    [SerializeField] KeyCode right = KeyCode.D;
    [SerializeField] KeyCode fire1 = KeyCode.Mouse0;
    [SerializeField] KeyCode fire2 = KeyCode.Mouse1;


    [Space]
    [Header("Big recoil")]
    KeyCode fireBigRecoil;
    [Range(5f, 500f)] [SerializeField] float recoilSize = 300f;
    [Header("Rocket Jump")]
    KeyCode fireRocket;
    [Range(5f, 500f)] [SerializeField] float rocketForce = 300f;
    [Range(.1f, 10f)] [SerializeField] float rocketRadius = 2f;
    [SerializeField] GameObject rocketPrefab;

    // Start is called before the first frame update
    void Start()
    {
        

        if (character == null)
        {
            character = gameObject;
        }

        if (rb == null)
        {
            rb = character.GetComponent<Rigidbody2D>();
        }

        if (cam == null)
        {
            cam = GameObject.FindObjectOfType<Camera>();
        }

        rb.gravityScale = gravityScale;
        //rb.mass = mass;

        //assign guns to fire buttons
        fireBigRecoil = fire1;
        fireRocket = fire2;
    }

    // Update is called once per frame
    void Update()
    {
        moveControls = Vector3.zero;
        //update controls here
        if (Input.GetKey(left))
        {
            //Debug.Log("walk left");
            moveControls.x += -walkSpeed * Time.deltaTime;
        }

        if (Input.GetKey(right))
        {
            //Debug.Log("walk Right");
            moveControls.x += walkSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(fireBigRecoil))
        {
            //if the big recoil has been fired apply big recoil
            //get the mouse position in world coordinates
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            //get the vector from mouse position to object position
            Vector2 direction = new Vector2(gameObject.transform.position.x - mousePos.x,gameObject.transform.position.y - mousePos.y).normalized;

            rb.AddForce(direction * recoilSize);
        }

        if (Input.GetKeyDown(fireRocket))
        {
            
            shootRocket();
        }

    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundchecker.transform.position, groundCheckRadius, ground);
        grounded = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
            }
        }

        if (grounded) //only walk on the ground, no air adjustments
        {
            Vector3 targetVelocity = new Vector3(moveControls.x * 10f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, movementSmoothing);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundchecker.transform.position, groundCheckRadius);
    }

    void shootRocket()
    {
        if (rocketPrefab != null)
        { 
            Vector3 dir = cam.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position;
            GameObject tempRocket = GameObject.Instantiate(rocketPrefab, gameObject.transform.position + dir.normalized *.1f, Quaternion.identity);
            tempRocket.transform.forward = dir.normalized;
            tempRocket.GetComponent<rocketScript>().Init(dir, rocketForce, rocketRadius);
        }
    }

}

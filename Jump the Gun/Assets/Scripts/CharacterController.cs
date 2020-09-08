using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
#region Variables
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
    public bool lastGrounded;
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
    [Range(1,5)][SerializeField] int maxBigRecoilShots = 2;
    int numBigRecoilShots = 0;
    [Header("Rocket Jump")]
    KeyCode fireRocket;
    [Range(5f, 500f)] [SerializeField] float rocketForce = 300f;
    [Range(.1f, 10f)] [SerializeField] float rocketRadius = 2f;
    [SerializeField] int maxRockets = 2;
    int numRockets = 0;
    [SerializeField] GameObject rocketPrefab;
    [Range(0f, 1f)] [SerializeField] float liftOffCheckTime = .2f;
    bool subtractRockets = false;
    bool subtractBigRecoil =false;
	#endregion

#region UnityFunctions
	// Start is called before the first frame update
	void Start()
    {

        GetObjects();

        rb.gravityScale = gravityScale;
        //rb.mass = mass;

        InitializeGuns();
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
            ShootBigRecoil();
           
        }

        if (Input.GetKeyDown(fireRocket))
        {
            //if the rocket has been fired shoot the rocket
            ShootRocket();
        }

    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundchecker.transform.position, groundCheckRadius, ground);
        lastGrounded = grounded;
        grounded = CheckGrounded(colliders);

        if (grounded) //only walk on the ground, no air adjustments
        {
            Vector3 targetVelocity = new Vector3(moveControls.x * 10f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, movementSmoothing);

            if (!lastGrounded)//if last frame you were not grounded this frame you have landed
            {
                Landed();
            }
        }
        else if (lastGrounded) //if you are not currently grounded but last frame you were you just took off
        {
            //check to see if you shot a rocket or big recoil recently then subtract them
            if (subtractRockets)
            { 
                numRockets -= 1;
                subtractRockets = false;
            }

            if (subtractBigRecoil)
            { 
                numBigRecoilShots -= 1;
                subtractBigRecoil = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundchecker.transform.position, groundCheckRadius);
    }

    private void OnGUI()
    {
        GUI.backgroundColor = Color.gray;
        GUI.contentColor = Color.yellow;
        GUI.Label(new Rect(10, 10, 300, 100), "Number of Bigrecoil Shots: " + numBigRecoilShots + "\nNumber of Rockets: " + numRockets);
    }
#endregion

#region guns

    /// <summary>
    /// shoots the a rocket in the direction of the mouse
    /// </summary>
    void ShootRocket()
    {
        if (numRockets < 1) //shoot no rockets if there arne't any left
        { return; }

        if (!grounded)
        {
            numRockets -= 1; //decrement the number of big recoil shots remaining
        }
        else
        {
            subtractRockets = true;
            StartCoroutine(removeRocketsFired());
        }


        if (rocketPrefab != null)
        { 
            Vector3 dir = cam.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position; //get the direction the rocket is going to be going in
            GameObject tempRocket = GameObject.Instantiate(rocketPrefab, gameObject.transform.position + dir.normalized *.1f, Quaternion.identity); //set the rocket
            tempRocket.transform.forward = dir.normalized; //set the rockets rotation
            tempRocket.GetComponent<rocketScript>().Init(dir, rocketForce, rocketRadius); //initialize the rocket
        }
    }


    /// <summary>
    /// shoots the gun that just propels the character away from the pointer
    /// </summary>
    void ShootBigRecoil()
    {
        if (numBigRecoilShots < 1)// no big recoil shots if there are no bullets left
        { return; }

        if (!grounded)
        {
            numBigRecoilShots -= 1; //decrement the number of big recoil shots remaining
        }
        else
        {
            subtractBigRecoil = true;
            StartCoroutine(removeBigRecoilFired());
        }


        //get the mouse position in world coordinates
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //get the vector from mouse position to object position
        Vector2 direction = new Vector2(gameObject.transform.position.x - mousePos.x, gameObject.transform.position.y - mousePos.y).normalized;

        rb.AddForce(direction * recoilSize);

    }

    /// <summary>
    /// sets the initial values of all of the guns
    /// </summary>
    void InitializeGuns()
    {
        //assign guns to fire buttons
        fireBigRecoil = fire1;
        fireRocket = fire2;

        //initialize the values of the guns
        ReloadGuns();

    }

    /// <summary>
    /// sets the number of all of the guns back to the maximum of guns
    /// </summary>
    void ReloadGuns()
    {
        numRockets = maxRockets;
        numBigRecoilShots = maxBigRecoilShots;
    }

	#endregion

#region initializationHelpers

	/// <summary>
	/// Gets the game object, rigidbody, and 
	/// </summary>
	void GetObjects()
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
    }
    #endregion

#region physicsHelpers
    bool CheckGrounded(Collider2D[] colliders)
    {
        bool ground = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                ground = true;
            }
        }
        return ground;
    }

    void Landed()
    {

        //landing causes you to reload your guns
        ReloadGuns();
    }

    #endregion
#region coroutines
    IEnumerator removeRocketsFired()
    {
        yield return new WaitForSeconds(liftOffCheckTime);
        subtractRockets = false;
    }
    IEnumerator removeBigRecoilFired()
    {
        yield return new WaitForSeconds(liftOffCheckTime);
        subtractBigRecoil = false;
    }
    #endregion


}

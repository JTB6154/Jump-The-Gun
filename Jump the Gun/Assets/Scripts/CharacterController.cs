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
    groundFinder finder;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask reloadLayer;
    [SerializeField] Texture2D cursorTexture;
    [Space]

    [Header("Physics settings")]

    [SerializeField] float gravityScale = 1f;
    [SerializeField] float walkSpeed = 5f;
    //[SerializeField] float mass = 2.5f;
    [Range(5f, 150f)] [SerializeField] float maxXSpeed = 9f;
    [Range(-150f, -5f)] [SerializeField] float minXSpeed = -9f;
    [Range(5f, 150f)] [SerializeField] float maxYSpeed = 9f;
    [Range(-150f,-5f)] [SerializeField] float minYSpeed = -9f;
    float maxSpeed;
    Vector3 moveControls = new Vector3();
    //Vector3 velocity = new Vector2(0, 0);
    //Vector3 smoothVelocity = Vector3.zero;
    //[Range(0f, .3f)] [SerializeField] float movementSmoothing = .05f; //currently unused, may be used later for time to reach max speed
    [SerializeField] public bool velocityZeroing = true;
    [SerializeField] float maxWalkableAngle = 1f;


    public bool grounded;
    public bool lastGrounded;
    [SerializeField] float groundCheckRadius = .15f;
    [Range(0.001f, .3f)] [SerializeField] float groundCheckDepth = .01f;
    [Range(0.001f, 1f)] [SerializeField] float groundTimer = .01f;

    [Space]
    [Header("Big recoil")]
    public bool hasBigRecoil = false;
    [Range(5f, 1500f)] [SerializeField] float recoilForce = 300f;
    [Range(1, 5)] [SerializeField] int maxBigRecoilShots = 2;
    [SerializeField] GameObject shotgunShot;
    [SerializeField] float maxVariation = 10f;
    [SerializeField] int numShot = 3;
    int numBigRecoilShots = 0;

    [Header("Rocket Jump")]
    public bool hasRocketJump = false;
    [Range(5f, 2000f)] [SerializeField] float rocketForce = 300f;
    [Range(.1f, 10f)] [SerializeField] float rocketRadius = 2f;
    [SerializeField] int maxRockets = 2;
    int numRockets = 0;
    [SerializeField] GameObject rocketPrefab;
    [Range(0f, 1f)] [SerializeField] float liftOffCheckTime = .2f;
    bool subtractRockets = false;
    bool subtractBigRecoil = false;

    public Animator animator;
    private SpriteRenderer sprite;
    private bool shooting;
    public bool standStill = false;
    private float animTimer;
    private float cutsceneTimer;
    private float cutsceneLength;
    private short gun;
    #endregion

    #region UnityFunctions
    void Start()
    {

        if (shotgunShot == null)
        {
            Debug.LogError("shotgun does not have proper shot");
        }
        
        GetObjects();
 
        rb.gravityScale = gravityScale;
        maxSpeed = new Vector2(maxXSpeed, maxYSpeed).magnitude;
        //rb.mass = mass;

        InitializeGuns();
        SetCursor();

        if (GameStats.Instance.hasSaveData == 1)
        {
            //Load data for player from saved
            this.transform.position = new Vector2(GameStats.Instance.playerPosX, GameStats.Instance.playerPosY);
            rb.velocity = new Vector2(GameStats.Instance.playerMomentumX, GameStats.Instance.playerMomentumX);
            numBigRecoilShots = GameStats.Instance.bigRecoilAmmo;
            numRockets = GameStats.Instance.rocketLauncherAmmo;
        }

        StartCutscene();
    }

    void Update()
    {
        moveControls = Vector3.zero;

        //update controls here

        if (grounded && !standStill)
        {
            //currently no smoothing between starting to walk and being walking.
            if (Input.GetKey(Options.Instance.Left))
            {
                //Debug.Log("walk left");
                moveControls.x += -walkSpeed;
            }

            if (Input.GetKey(Options.Instance.Right))
            {
                //Debug.Log("walk Right");
                moveControls.x += walkSpeed;
            }
        }

        if (Input.GetKeyDown(Options.Instance.Fire1))
        {
            //if the big recoil has been fired apply big recoil
            ShootBigRecoil();

        }

        if (Input.GetKeyDown(Options.Instance.Fire2))
        {
            //if the rocket has been fired shoot the rocket
            ShootRocket();
        }

        //Update the player character's animations based on their movement
        UpdateAnim();

        //update the cursor

        //Update gamestats
        GameStats.Instance.bigRecoilAmmo = numBigRecoilShots;
        GameStats.Instance.rocketLauncherAmmo = numRockets;
        GameStats.Instance.playerPosX = this.transform.position.x;
        GameStats.Instance.playerPosY = this.transform.position.y;
        GameStats.Instance.playerMomentumX = rb.velocity.x;
        GameStats.Instance.playerMomentumY = rb.velocity.y;

    }

    private void FixedUpdate()
    {
        lastGrounded = grounded;
        grounded = CheckGrounded(ground);

        if (grounded) //only walk on the ground, no air adjustments
        {
            Vector3 targetVelocity = new Vector3(moveControls.x, rb.velocity.y);
            //Debug.Log(targetVelocity);
            targetVelocity = targetVelocity.normalized * Mathf.Clamp(targetVelocity.magnitude, 0f, maxSpeed); //clamps the speed to between zero and max speed
            //rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, movementSmoothing);
            //Debug.Log(targetVelocity);
            rb.velocity = targetVelocity;

            //if (!lastGrounded)//if last frame you were not grounded this frame you have landed
            //{
            //    //Landed(); //currently has no functionality
            //}
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

        

        //cap the speed of the player in the x and y directions
        if (rb.velocity.x > maxXSpeed)
        {
            rb.velocity = new Vector2(maxXSpeed, rb.velocity.y);
        }

        if (rb.velocity.x < minXSpeed)
        {
            rb.velocity = new Vector2(minXSpeed, rb.velocity.y);

        }


        if (rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxYSpeed);
        }

        if (rb.velocity.y < minYSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, minYSpeed);
        }

        if (standStill)
        {
            cutsceneTimer += Time.deltaTime;
            if (cutsceneTimer >= cutsceneLength) standStill = false;
        }
    }



    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(groundchecker.transform.position, groundCheckRadius); //old ground checking, depreciated
        Gizmos.DrawLine(groundchecker.transform.position + -transform.right * groundCheckRadius,
                        groundchecker.transform.position + -transform.right * groundCheckRadius + -Vector3.up * groundCheckDepth);
        Gizmos.DrawLine(groundchecker.transform.position + transform.right * groundCheckRadius,
                groundchecker.transform.position + transform.right * groundCheckRadius + -Vector3.up * groundCheckDepth);
        Gizmos.DrawLine(groundchecker.transform.position,
                        groundchecker.transform.position + -Vector3.up * groundCheckDepth);
    }

    private void OnGUI()
    {
        GUI.backgroundColor = Color.gray;
        GUI.contentColor = Color.yellow;
        GUI.Label(new Rect(10, 10, 300, 100), "Number of Bigrecoil Shots: " + numBigRecoilShots + "\nNumber of Rockets: " + numRockets);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (character.GetComponent<Collider2D>().IsTouchingLayers(reloadLayer))
        {
            ReloadGuns();
        }
    }

    #endregion

    #region guns


    public int GetBigRecoilAmmo()
    {
        return numBigRecoilShots;
    }

    public int GetRocketAmmo()
    {
        return numRockets;
    }
    /// <summary>
    /// shoots the a rocket in the direction of the mouse
    /// </summary>
    void ShootRocket()
    {
        if (hasRocketJump && GameStats.Instance.isPaused == false)
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
                //float angle = Mathf.Atan2(dir.y, dir.x);
                GameObject tempRocket = Instantiate(rocketPrefab, gameObject.transform.position + dir.normalized * .1f, Quaternion.identity); //set the rocket
                tempRocket.transform.forward = dir.normalized; //set the rockets rotation
                tempRocket.GetComponent<rocketScript>().Init(dir, rocketForce, rocketRadius); //initialize the rocket

                //Startup the shooting animation
                StartShootAnim(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, 1);
            }
        }
    }


    /// <summary>
    /// shoots the gun that just propels the character away from the pointer
    /// </summary>
    void ShootBigRecoil()
    {

        if (hasBigRecoil && GameStats.Instance.isPaused == false)
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

            if (velocityZeroing)
            {
                rb.velocity = Vector2.zero;
            }

            //get the mouse position in world coordinates
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            //get the vector from mouse position to object position
            Vector2 direction = new Vector2(gameObject.transform.position.x - mousePos.x, gameObject.transform.position.y - mousePos.y).normalized;

            rb.AddForce(direction * recoilForce);
            
            if (shotgunShot != null)
            {
                for (int i = 0; i < numShot; i++)
                {
                    GameObject shooting = GameObject.Instantiate(shotgunShot);
                    shooting.transform.position = new Vector3(gameObject.transform.position.x - (2 * direction.x), gameObject.transform.position.y - (2 * direction.y), gameObject.transform.position.z);
                    shooting.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg + Random.Range(-maxVariation, maxVariation));
                }
            }

            //Startup the shooting animation
            StartShootAnim(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, 0);
        }
    }

    /// <summary>
    /// sets the initial values of all of the guns
    /// </summary>
    void InitializeGuns()
    {
        //initialize the values of the guns
        if (GameStats.Instance.hasSaveData == 0)
        {
            ReloadGuns();
        }

    }

    //private Vector2 GetVisualDirection(Vector2 direction)
    //{
    //    float x = 0, y = 0;
    //    float angle = Mathf.Atan2(direction.y, direction.y) * Mathf.Rad2Deg;

    //    return new Vector2(x, y);
    //}

    /// <summary>
    /// sets the number of all of the guns back to the maximum of guns
    /// </summary>
    public void ReloadGuns()
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
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        finder = groundchecker.GetComponent<groundFinder>();
    }
    #endregion

    #region physicsHelpers
    bool CheckGrounded(LayerMask countsAsGround)
    {

        bool isGrounded = false;

        //Collider2D[] colliders = Physics2D.OverlapCircleAll(groundchecker.transform.position, groundCheckRadius, countsAsGround);
        RaycastHit2D right = Physics2D.Raycast(groundchecker.transform.position + transform.right * groundCheckRadius, -Vector2.up, groundCheckDepth, countsAsGround);
        RaycastHit2D left = Physics2D.Raycast(groundchecker.transform.position + -transform.right * groundCheckRadius, -Vector2.up, groundCheckDepth, countsAsGround);
        RaycastHit2D center = Physics2D.Raycast(groundchecker.transform.position, -Vector2.up, groundCheckDepth, countsAsGround);


        if (finder.getGrounded(countsAsGround))
        {

            isGrounded = true;

            if (!CheckRayCastNormalAngle(left)) isGrounded = false;
            if (!CheckRayCastNormalAngle(right)) isGrounded = false;
            if (!CheckRayCastNormalAngle(center)) isGrounded = false;

        }


        return isGrounded;
    }

    bool CheckRayCastNormalAngle(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            //Debug.Log(Mathf.Abs(Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg));
            if (Mathf.Abs(Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg) > maxWalkableAngle)
            {
                return false;
            }
        }

        return true;
    }

    void Landed()
    {
        //no functions here yet
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

    IEnumerator landedDoubleCheck()
    {
        yield return new WaitForSeconds(groundTimer);
        if (grounded) ReloadGuns();
    }
    #endregion

    #region AnimationHelpers


    //Determine what animation needs to be played
    //based on what the player character is currently doing
    private void UpdateAnim()
    {
        if (standStill) return;

        //Check to see if the player has finished the shooting animation
        if (shooting && animTimer <= Time.time)
        {
            if (gun == 0) //shotgun
                animator.SetBool("Shotgun", false);
            else //rocket
                animator.SetBool("Rocket", false);
            shooting = false;
        }

        //Check if the player is moving up or down
        if (!grounded)
            animator.SetBool("Jumping", true);
        else
            animator.SetBool("Jumping", false);

        //Check if the player character is moving left or right
        if (rb.velocity.x > 0 && grounded && !shooting)
        {
            sprite.flipX = true;
            animator.SetBool("Running", true);
        }
        else if (rb.velocity.x < 0 && grounded && !shooting)
        {
            sprite.flipX = false;
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }

    //Start the animation for shooting, and
    //setup variables to track when the animation ends
    private void StartShootAnim(float angle, short gun_)
    {
        gun = gun_;
        if (gun == 0) //shotgun
        {
            if (animator.GetBool("Rocket"))
                animator.SetBool("Rocket", false);
            animator.SetBool("Shotgun", true);
            angle *= -1;
            if (angle > 100 || angle < -100)
            {
                sprite.flipX = true;
                float sign = angle / Mathf.Abs(angle);
                angle = (180 - Mathf.Abs(angle)) * sign;
            }
            else
            {
                sprite.flipX = false;
            }
            animator.SetFloat("Angle", angle);
        }
        else //rocket
        {
            if (animator.GetBool("Shotgun"))
                animator.SetBool("Shotgun", false);
            animator.SetBool("Rocket", true);
            if (angle <= 100 && angle >= -100)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
                float sign = angle / Mathf.Abs(angle);
                angle = (180 - Mathf.Abs(angle)) * sign;
            }
            animator.SetFloat("Angle", angle);
        }

        //Set the timer

        int timerOffset = 40;
        if (animator.GetBool("Running")) { timerOffset = 10; }
        animTimer = Time.time + (timerOffset * Time.deltaTime);
        shooting = true;
    }

    public void StartCutscene()
    {
        standStill = true;
        cutsceneLength = animator.GetCurrentAnimatorStateInfo(0).length;
        cutsceneTimer = 0;
        animator.SetBool("Jumping", false);
        animator.SetBool("Running", false);
    }

    void SetCursor()
    {
        if (cursorTexture != null)
        { 

            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);

        }
    }
    #endregion
}

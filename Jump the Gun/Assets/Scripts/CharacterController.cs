﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Variables
    [Space]
    [Header("1 - CHARACTER OBJECTS")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject character;
    [SerializeField] Camera cam;
    [SerializeField] GameObject groundchecker;
    GroundFinder finder;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask reloadLayer;
    [SerializeField] private List<FiringPoint> firingPoints;
    
    [Space]
    [Header("2 - PHYSICS SETTINGS")]
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
    [SerializeField] float speedInAir;
    [SerializeField] DynamicBusVolumeController airTravelBusVolumeController;

    [Space]
    [Header("3.1 - BIG RECOIL")]
    public bool hasBigRecoil = false;
    [Range(5f, 1500f)] [SerializeField] float recoilForce = 300f;
    [Range(1, 5)] [SerializeField] int maxBigRecoilShots = 2;
    [SerializeField] GameObject shotgunEffect;
    [SerializeField] GameObject shotgunBullet;
    [SerializeField] float maxVariation = 10f;
    //[SerializeField] int numShot = 3;
    int numBigRecoilShots = 0;

    [Space]
    [Header("3.2 - ROCKET JUMP")]
    public bool hasRocketJump = false;
    [Range(5f, 2000f)] [SerializeField] float rocketForce = 300f;
    [Range(.1f, 10f)] [SerializeField] float rocketRadius = 2f;
    [SerializeField] int maxRockets = 2;
    int numRockets = 0;
    [SerializeField] GameObject rocketPrefab;
    [Range(0f, 1f)] [SerializeField] float liftOffCheckTime = .2f;
    bool subtractRockets = false;
    bool subtractBigRecoil = false;

    [Space]
    [Header("4 - MISC SETTINGS")]
    public Animator animator;
    private SpriteRenderer sprite;
    private bool shooting;
    public bool standStill = false;
    private float animTimer;
    private float cutsceneTimer;
    private float cutsceneLength;
    private short gun;

    [Space]
    [Header("5 - AUDIO SETTINGS")]
    [FMODUnity.EventRef]
    public string ambient1Event;
    [FMODUnity.EventRef]
    public string landingEvent;
    [FMODUnity.EventRef]
    public string rocketShootingEvent;
    [FMODUnity.EventRef]
    public string shotgunShootingEvent;
    [FMODUnity.EventRef]
    public string rocketTravelingEvent;
    [FMODUnity.EventRef]
    public string walkingEvent;
    [FMODUnity.EventRef]
    public string movingThroughAirEvent;

    private List<Vector3> firingPointsTest = new List<Vector3>();

    #endregion

    #region UnityFunctions
    void Start()
    {
        if (shotgunEffect == null)
        {
            Debug.LogError("shotgun does not have proper shot");
        }
        
        GetObjects();
 
        rb.gravityScale = gravityScale;
        maxSpeed = new Vector2(maxXSpeed, maxYSpeed).magnitude;
        //rb.mass = mass;

        InitializeGuns();

        if (GameStats.Instance.hasSaveData == 1)
        {
            //Load data for player from saved
            this.transform.position = new Vector2(GameStats.Instance.playerPosX, GameStats.Instance.playerPosY);
            rb.velocity = new Vector2(GameStats.Instance.playerMomentumX, GameStats.Instance.playerMomentumY);
            numBigRecoilShots = GameStats.Instance.bigRecoilAmmo;
            numRockets = GameStats.Instance.rocketLauncherAmmo;

            animator.SetBool("Initialized", true);
            if (GameStats.Instance.hasBigRecoil == 1)
            {
                animator.SetBool("Gunless", false);
                hasBigRecoil = true;
            }
            if (GameStats.Instance.hasRocketLauncher == 1)
            {
                animator.SetBool("Gunless", false);
                hasRocketJump = true;
            }
        }

        AudioManager.Instance.StartMusic(ambient1Event);

        sprite.flipX = true;
        StartCutscene();
    }

    void Update()
    {
        moveControls = Vector3.zero;

        //update controls here

        if (grounded && !standStill)
        {
            AudioManager.Instance.SetPlayerSoundsBusMute(false);

            //currently no smoothing between starting to walk and being walking.
            if (Input.GetKey(Options.Instance.Left))
            {
                //Debug.Log("walk left");
                moveControls.x += -walkSpeed;
                AudioManager.Instance.PlayLoop(walkingEvent);
            }
            else if (Input.GetKey(Options.Instance.Right))
            {
                //Debug.Log("walk Right");
                moveControls.x += walkSpeed;
                AudioManager.Instance.PlayLoop(walkingEvent);
            }
            else if (Input.GetKeyUp(Options.Instance.Left) || Input.GetKeyUp(Options.Instance.Right))
            {
                AudioManager.Instance.StopLoop(walkingEvent, SoundBus.PlayerSounds);
            }
        }
        else
        {
            AudioManager.Instance.StopLoop(walkingEvent, SoundBus.PlayerSounds);
        }

        if (Input.GetKeyDown(Options.Instance.Fire1) && !standStill)
        {
            //if the big recoil has been fired apply big recoil
            ShootBigRecoil();
        }

        if (Input.GetKeyDown(Options.Instance.Fire2) && !standStill)
        {
            //if the rocket has been fired shoot the rocket
            ShootRocket();
        }

        //Update the player character's animations based on their movement
        UpdateAnim();

        //update the cursor

        //Update gamestats
        if (hasBigRecoil)
        {
            GameStats.Instance.hasBigRecoil = 1;
        }
        if (hasRocketJump)
        {
            GameStats.Instance.hasRocketLauncher = 1;
        }
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

            if (!lastGrounded)//if last frame you were not grounded this frame you have landed
            {
                Landed(); //currently has no functionality
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
                subtractBigRecoil = false;
            }
        }

        if (!grounded) // In air
        {
            // Update speed in air
            //Debug.Log("In air");
            speedInAir = Mathf.Sqrt(Vector2.SqrMagnitude(rb.velocity));

            if (rb.velocity.y < -0.1f)
            {
                // Is falling
                AudioManager.Instance.PlayLoop(movingThroughAirEvent);
                AudioManager.Instance.SetDynamicBusVolume(airTravelBusVolumeController, speedInAir);
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

        // Draw arcs every 10 degree angle
        //Gizmos.color = Color.white;
        //float gizmosRadius = 10f;
        //int unitAngle = 15;
        //Gizmos.DrawWireSphere(transform.position, gizmosRadius);
        //for (int i = 0; i < 360 / unitAngle; i++)
        //{
        //    float angle = i * unitAngle * Mathf.Deg2Rad;
        //    Gizmos.DrawLine(transform.position, 
        //        transform.position + new Vector3(Mathf.Cos(angle) * gizmosRadius, Mathf.Sin(angle) * gizmosRadius));
        //}
    }

    private void OnGUI()
    {
        if (GameStats.Instance.isShotNumberGUIOn)
        {
            GUI.backgroundColor = Color.gray;
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect(10, 10, 300, 100), "Number of Bigrecoil Shots: " + numBigRecoilShots + "\nNumber of Rockets: " + numRockets);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (character.GetComponent<Collider2D>().IsTouchingLayers(reloadLayer))
        {
            ReloadGuns();
        }
    }

    #endregion

    #region Guns


    public int GetBigRecoilAmmo()
    {
        return numBigRecoilShots;
    }

    public int GetRocketAmmo()
    {
        return numRockets;
    }

    private Vector3 GetFiringPoint(float firingAngle, bool isGun)
    {
        Vector3 output = Vector3.zero;
        bool isFlipped = false;

        if (isGun)
        {
            firingAngle *= -1;

            if (firingAngle > 100 || firingAngle < -100)
            {
                isFlipped = true;
                int sign = firingAngle > 0 ? 1 : -1;
                firingAngle = sign * (180 - Mathf.Abs(firingAngle));
            }
        }
        else
        {
            // If shooting a rocket
            if (firingAngle > 100 || firingAngle < -100)
            {
                int sign = firingAngle > 0 ? 1 : -1;
                firingAngle = sign * (180 - Mathf.Abs(firingAngle));
            }
            else
                isFlipped = true;
        }

        for (int i = 0; i < firingPoints.Count; i++)
        {
            if (firingPoints[i].IsWithinRange(firingAngle))
            {
                output = isFlipped ?
                    firingPoints[i].GetFlippedFiringPosition() : firingPoints[i].GetFiringPosition();

                //string flip = isFlipped ? "flipped" : "";
                //Debug.Log("Firing from " + firingPoints[i].firingPointTransform.gameObject.name + " " + flip);
                break;
            }
        }

        return output;
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
                float firingAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //GameObject tempRocket = Instantiate(rocketPrefab, gameObject.transform.position + dir.normalized * .1f, Quaternion.identity); //set the rocket
                Vector3 firingPoint = GetFiringPoint(firingAngle, false);

                Vector3 dir2 = firingPoint - transform.position;
                dir2.z = 0f;
                
                GameObject tempRocket = Instantiate(rocketPrefab, firingPoint, Quaternion.identity); //set the rocket
                tempRocket.transform.forward = dir.normalized; //set the rockets rotation
                tempRocket.GetComponent<RocketScript>().Init(dir, rocketForce, rocketRadius); //initialize the rocket
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), dir2, dir2.magnitude, tempRocket.GetComponent<RocketScript>().notPlayer);

                if (hit)
                {
                    Debug.Log("The rocket hits " + hit.transform.gameObject.name);
                    tempRocket.transform.position = new Vector3(hit.point.x, hit.point.y);
                    tempRocket.GetComponent<RocketScript>().Explode();
                    //Startup the shooting animation
                    StartShootAnim(firingAngle, 1);

                    return;
                }

                //Startup the shooting animation
                StartShootAnim(firingAngle, 1);

                // Audio code
                AudioManager.Instance.PlayOneShot(rocketShootingEvent);
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

            float firingAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Startup the shooting animation
            StartShootAnim(firingAngle, 0);

            GameObject shooting = null;
            if (shotgunEffect != null)
            {
                shooting = Instantiate(shotgunEffect);
                shooting.transform.position = GetFiringPoint(firingAngle, true);
                shooting.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);
            }

            firingAngle -= 5;
            for (int i = 0; i < 4; i++)
            {
                firingAngle += Random.Range(1.5f, 2.5f);
                ShotgunBullet bullet = Instantiate(shotgunBullet).GetComponent<ShotgunBullet>();
                bullet.transform.position = shooting.transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);
                bullet.Init(new Vector2(-Mathf.Cos(firingAngle * Mathf.Deg2Rad), -Mathf.Sin(firingAngle * Mathf.Deg2Rad)).normalized);
            }

            // Audio code
            AudioManager.Instance.PlayOneShot(shotgunShootingEvent);
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

    #region InitializationHelpers

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

        finder = groundchecker.GetComponent<GroundFinder>();
    }
    #endregion

    #region PhysicsHelpers
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
            float min = float.MaxValue;

            if (min > CheckRayCastNormalAngle(left)) min = CheckRayCastNormalAngle(left);
            if (min > CheckRayCastNormalAngle(right)) min = CheckRayCastNormalAngle(right);
            if (min > CheckRayCastNormalAngle(center)) min = CheckRayCastNormalAngle(center);

            if (min > maxWalkableAngle && min != float.MaxValue) isGrounded = false;
        }

        return isGrounded;
    }

    float CheckRayCastNormalAngle(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            //Debug.Log(Mathf.Abs(Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg));
            return Mathf.Abs(Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg);
            
        }

        return float.MaxValue;
    }

    void Landed()
    {
        speedInAir = 0f;
        AudioManager.Instance.PlayOneShot(landingEvent);
        AudioManager.Instance.StopLoop(movingThroughAirEvent, SoundBus.AirTravel);
    }

    #endregion

    #region Coroutines
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
        if (shooting)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                if (gun == 0) //shotgun
                    animator.SetBool("Shotgun", false);
                else //rocket
                    animator.SetBool("Rocket", false);
                shooting = false;
            }
        }

        //Check if the player is moving up or down
        if (!grounded)
            animator.SetBool("Jumping", true);
        else
            animator.SetBool("Jumping", false);

        //Check if the player character is moving left or right
        if (Input.GetKey(Options.Instance.Left) && grounded && !shooting)
        {
            sprite.flipX = false;
            animator.SetBool("Running", true);
        }
        else if (Input.GetKey(Options.Instance.Right) && grounded && !shooting)
        {
            sprite.flipX = true;
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
        animTimer = 0;
        if (animator.GetBool("Running")) animTimer += 0.1f;
        shooting = true;
    }

    public void StartCutscene()
    {
        standStill = true;
        animator.SetBool("Jumping", false);
        animator.SetBool("Running", false);
        cutsceneLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
        cutsceneTimer = 0;
    }
    #endregion

}

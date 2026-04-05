using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject cam;
    public GameObject head;
    public GameObject bombPrefab;
    public GameObject bombSpawn;
    public GameObject bombPivot;
    public LayerMask groundLayer;
    public GameObject playerSpawner;
    public GameObject world;
    public GameObject interactBillboard;

    private InputAction moveAction;
    private Vector2 moveValue;
    private InputAction jumpAction;
    private InputAction aimAction;
    private InputAction throwAction;
    private InputAction interactAction;
    private LineRenderer lr;
    private Vector3 lastPosition;

    private float speed = 300f;
    private float jumpForce = 12f;
    private float jumpMult = 2f;
    private float groundDistance = .4f;
    private float playerHeight = 2f;

    public int ammo = 0;

    private bool isGrounded = false;
    private bool lastGrounded = false;
    private bool isRunning = false;
    private bool isAiming = false;
    private bool isThrowing = false;
    private bool isInteracting = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isInAir = false;
    private bool isSpawning = true;
    private bool isDying = false;

    private Vector3 lookDirection;
    private Vector3 throwHeadDirection;
    private Vector3 bombDirection;
    private RaycastHit slopeHit;

    private Rigidbody rb;
    private Animator anim;
    private GameObject model;
    private GameObject hitbox;
    private GameObject spawnPoint;
    private GameObject tempBomb;

    [SerializeField] private TMP_Text ammoDisplay;
    [SerializeField] private AudioSource sfxManager;
    [SerializeField] private AudioClip explodeSFX;
    
    [HideInInspector] public bool canInteract = false;
    [HideInInspector] public GameObject currentInteractable;
    [HideInInspector] public bool isBombJumping = false;
    [HideInInspector] public Vector3 bombForce = new Vector3(0,0,0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.SetParent(world.transform); 
        rb = GetComponent<Rigidbody>();
        model = transform.Find("Model").gameObject;
        anim = model.GetComponent<Animator>();
        hitbox = transform.Find("Hitbox").gameObject;
        lr = GetComponent<LineRenderer>();
        
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        aimAction = InputSystem.actions.FindAction("Aim");
        throwAction = InputSystem.actions.FindAction("Attack");
        interactAction = InputSystem.actions.FindAction("Interact");

        lookDirection = model.transform.forward;

        // Spawn on Spawn Point
        spawnPoint = playerSpawner.transform.Find("SpawnPoint").gameObject;
        SpawnPlayer(spawnPoint.transform.position);

        lastPosition = spawnPoint.transform.position;
        StartCoroutine(LastPositionCheck(3f));
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is on ground
        isGrounded = Physics.Raycast(transform.position + new Vector3(0,playerHeight*.5f,0), Vector3.down, playerHeight * .5f + groundDistance, groundLayer);

        // Stop actions if spawning/dying animation
        if (!isSpawning && !isDying)
        {
            // Read WASD
            moveValue = moveAction.ReadValue<Vector2>();

            // Rotate model to move direction
            if (moveAction.IsPressed() && !isInteracting)
            {
                isRunning = true;
                lookDirection = Vector3.RotateTowards(lookDirection, new Vector3(moveValue.x, 0, moveValue.y), 15f * Time.deltaTime, 0f);
                model.transform.LookAt(new Vector3(model.transform.position.x + lookDirection.x, model.transform.position.y, model.transform.position.z + lookDirection.z));
            }
            else
            {
                isRunning = false;
            }

            // Stop actions if interacting
            if (!isInteracting)
            {
                // Play run animation
                if (moveAction.IsPressed() && isGrounded && !anim.GetCurrentAnimatorStateInfo(0).IsName("Running") && !anim.GetNextAnimatorStateInfo(0).IsName("Running") && !jumpAction.IsPressed())
                {
                    anim.CrossFadeInFixedTime("Running", .1f, 0);
                }

                // Back to idle animation
                if (moveAction.WasReleasedThisFrame() && isGrounded)
                {
                    anim.CrossFadeInFixedTime("Idle", .1f, 0);
                }
                
                // Jump
                if (jumpAction.IsPressed() && isGrounded && !isInAir && !isJumping && !isFalling)
                {
                    isInAir = true;
                    isJumping = true;
                    anim.CrossFadeInFixedTime("Jump_Start", .1f, 0);
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
                
                // Start/stop aiming
                if (aimAction.WasPressedThisFrame() && ammo > 0 && !isAiming && !isThrowing)
                {
                    isAiming = true;
                    anim.SetFloat("ThrowSpeed", 1);
                    bombPivot.SetActive(true);
                    lr.enabled = true;
                    lr.positionCount = 2;
                }
                else if (aimAction.WasReleasedThisFrame() && isAiming && !isThrowing)
                {
                    isAiming = false;
                    anim.SetFloat("ThrowSpeed", -1);
                    bombPivot.SetActive(false);
                    lr.enabled = false;
                    lr.positionCount = 0;
                }
                
                // Throw bomb
                if (throwAction.WasPressedThisFrame() && isAiming && !isThrowing)
                {
                    isThrowing = true;
                    anim.SetFloat("ThrowSpeed", 1);
                    lr.enabled = false;
                    lr.positionCount = 0;
                }
                
                // Reset animation to idle (failsafe)
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Running") && !isRunning && !anim.GetNextAnimatorStateInfo(0).IsName("Idle"))
                {
                    anim.CrossFadeInFixedTime("Idle", .1f, 0); 
                }
            }

            // Falling off ledge
            if ((rb.linearVelocity.y < -5f || rb.linearVelocity.y > 5f)&& !isJumping && !isFalling && !isInAir && !OnSlope() && !isGrounded)
            {
                isInAir = true;
                isFalling = true;
                anim.CrossFadeInFixedTime("Jump_Idle", .1f, 0);
            }

            // On player landing
            if (isGrounded && !lastGrounded)
            {
                // Reset jump related bools
                isInAir = false;
                isJumping = false;
                isFalling = false;
                isBombJumping = false;

                // Reset velocity
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                // Run animation if WASD, else landing animation
                if (isRunning && !jumpAction.IsPressed())
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                    {
                        anim.CrossFadeInFixedTime("Running", .1f, 0); 
                    }
                }
                else
                {
                    anim.CrossFadeInFixedTime("Jump_Land", .1f, 0);
                }
            }

            // Interacting with lever/Big Bomb
            if (interactAction.WasPressedThisFrame() && canInteract && isGrounded && !isAiming && !isThrowing && !isInteracting && currentInteractable != null)
            {
                isInteracting = true;
                rb.linearVelocity = new Vector3(0,0,0);
                model.transform.LookAt(new Vector3(currentInteractable.transform.position.x, model.transform.position.y, currentInteractable.transform.position.z));
                anim.CrossFadeInFixedTime("Interact", .1f, 0);

                if (currentInteractable.CompareTag("Levers"))
                {
                    currentInteractable.GetComponent<LeverTrigger>().Trigger();
                }
                else if (currentInteractable.CompareTag("MegaBomb"))
                {
                    tempBomb = currentInteractable;
                    StartCoroutine(TriggerBigBombAfterTime(3f));
                    currentInteractable = null;
                    interactBillboard.SetActive(false);
                }

            }

            if (currentInteractable != null)
            {
                interactBillboard.SetActive(true);
                interactBillboard.transform.position = currentInteractable.transform.position;
                interactBillboard.transform.LookAt(Camera.main.transform.position);
            }
            else
            {
                interactBillboard.SetActive(false);
            }
        }

        // Store previous Grounded state
        lastGrounded = isGrounded;
    }
    
    void FixedUpdate()
    {
        // Stop actions if interacting/spawning/dying
        if (!isInteracting && !isSpawning && !isDying)
        {
            // Slope movement
            if (OnSlope())
            {
                rb.useGravity = false;
                rb.linearVelocity = GetSlopeMoveDirection() * speed * Time.deltaTime;
            }
            // Bomb jumping movement
            else if (isBombJumping)
            {
                rb.useGravity = true;
                rb.linearVelocity = new Vector3(Mathf.MoveTowards(rb.linearVelocity.x, 0, 10f * Time.deltaTime), rb.linearVelocity.y, Mathf.MoveTowards(rb.linearVelocity.z, 0, 10f * Time.deltaTime));
            }
            // Normal movement
            else
            {
                rb.useGravity = true;
                rb.linearVelocity = new Vector3(moveValue.x * speed * Time.deltaTime, rb.linearVelocity.y, moveValue.y * speed * Time.deltaTime);
            }
        }

        // Smoother jump/land
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * jumpMult * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * jumpMult * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // Make head look at aiming position
        if (isAiming)
        {
            if (!isThrowing)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                
                if (Physics.Raycast(ray, out hit, 100f, groundLayer)) 
                {
                    Transform objectHit = hit.transform;
                    bombDirection = (hit.point - bombSpawn.transform.position).normalized;
                    throwHeadDirection = new Vector3(hit.point.x, head.transform.position.y, hit.point.z);
                    
                    lr.SetPosition(0, bombSpawn.transform.position);
                    lr.SetPosition(1, hit.point);
                }
                else
                {
                    bombDirection = (ray.GetPoint(100f) - bombSpawn.transform.position).normalized;
                    throwHeadDirection = new Vector3(ray.GetPoint(100f).x, head.transform.position.y, ray.GetPoint(100f).z);

                    lr.SetPosition(0, bombSpawn.transform.position);
                    lr.SetPosition(1, ray.GetPoint(100f));
                }
            }
            else
            {
                throwHeadDirection = new Vector3(throwHeadDirection.x, head.transform.position.y, throwHeadDirection.z);
            }

            head.transform.LookAt(throwHeadDirection);
        }
    }
    

    public void StopMidThrow()
    {
        if (!isThrowing && anim.GetFloat("ThrowSpeed") > 0)
        {
            anim.SetFloat("ThrowSpeed", 0);
        }
    }
    

    public void StopStartThrow()
    {
        if (anim.GetFloat("ThrowSpeed") < 0)
        {
            anim.SetFloat("ThrowSpeed", 0);
        }
    }
    

    public void ResetThrow()
    {
        if (isThrowing)
        {
            isThrowing = false;
            anim.SetFloat("ThrowSpeed", 0);

            if (aimAction.IsPressed() && ammo > 0)
            {
                isAiming = true;
                anim.SetFloat("ThrowSpeed", 1);
                bombPivot.SetActive(true);
                lr.enabled = true;
                lr.positionCount = 2;
            }
            else
            {
                isAiming = false;
            }
        }
    }
    

    public void ThrowBomb()
    {
        ammo -= 1;
        UpdateAmmoDisplay();
        bombPivot.SetActive(false);
        lr.enabled = false;
        lr.positionCount = 0;

        GameObject bomb = Instantiate(bombPrefab, bombSpawn.transform.position, bombSpawn.transform.rotation);
        bomb.GetComponent<Rigidbody>().AddForce(bombDirection * 20f, ForceMode.Impulse);
        bomb.transform.LookAt(bomb.transform.position + (bombDirection * 20f));
    }
    

    public void Reset()
    {
        isInteracting = false;
        if (moveAction.IsPressed())
        {
            anim.CrossFadeInFixedTime("Running", .1f, 0);
        }
        else
        {
            anim.CrossFadeInFixedTime("Idle", .1f, 0);
        }
    }
    

    public void EndSpawn()
    {
        isSpawning = false;
    }


    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, .3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < 60 && angle != 0;
        }

        return false;
    }


    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(new Vector3(moveValue.x, 0, moveValue.y), slopeHit.normal).normalized;
    }


    public void SpawnPlayer(Vector3 spawn_position)
    {
        isSpawning = true;
        isDying = false;
        model.SetActive(true);
        anim.CrossFadeInFixedTime("Spawn_Ground", .1f, 0);
        anim.CrossFadeInFixedTime("Throw", .1f, 1);
        transform.position = spawn_position;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
    }

    public void ResetBombJump()
    {
        isBombJumping = false;
    }


    public void Die()
    {
        isDying = true;
        rb.linearVelocity = new Vector3(0,0,0);
        anim.CrossFadeInFixedTime("Death_A", .1f, 0);
        anim.CrossFadeInFixedTime("Idle", .1f, 1);

        isRunning = false;
        isAiming = false;
        isThrowing = false;
        isInteracting = false;
        isJumping = false;
        isFalling = false;
        isInAir = false;
        bombPivot.SetActive(false);
        lr.enabled = false;
        lr.positionCount = 0;

        StartCoroutine(ReloadSceneAfterTime(3f));
    }
    

    IEnumerator ReloadSceneAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void UpdateAmmoDisplay()
    {
        ammoDisplay.text = ammo.ToString();
    }
    

    IEnumerator TriggerBigBombAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        tempBomb.GetComponent<MegaBomb>().Triggered();
        currentInteractable = null;
        interactBillboard.SetActive(false);
    }
    

    IEnumerator LastPositionCheck(float time)
    {
        yield return new WaitForSeconds(time);

        if (isGrounded)
        {
            lastPosition = transform.position;
        }
        StartCoroutine(LastPositionCheck(time));
    }


    public void LavaDie()
    {
        isDying = true;
        rb.linearVelocity = new Vector3(0,0,0);
        anim.CrossFadeInFixedTime("Death_A", .1f, 0);
        anim.CrossFadeInFixedTime("Idle", .1f, 1);
        anim.SetFloat("ThrowSpeed", -1);

        isRunning = false;
        isAiming = false;
        isThrowing = false;
        isInteracting = false;
        isJumping = false;
        isFalling = false;
        isInAir = false;
        bombPivot.SetActive(false);
        lr.enabled = false;
        lr.positionCount = 0;

        StartCoroutine(SpawnAfterTime(3f, lastPosition));
    }
    

    IEnumerator SpawnAfterTime(float time, Vector3 spawn_pos)
    {
        yield return new WaitForSeconds(time);

        SpawnPlayer(spawn_pos);
    }
}

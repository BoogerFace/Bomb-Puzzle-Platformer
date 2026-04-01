using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public GameObject cam;
    public GameObject head;
    public GameObject bombSpawn;
    public GameObject bombPrefab;
    public LayerMask groundLayer;
    public GameObject playerSpawner;

    private InputAction moveAction;
    private Vector2 moveValue;
    private InputAction jumpAction;
    private InputAction aimAction;
    private InputAction throwAction;
    private InputAction interactAction;

    private float speed = 300f;
    private float jumpForce = 12f;
    private float jumpMult = 2f;
    private float groundDistance = .4f;
    private float playerHeight = 2f;

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
    

    private Vector3 lookDirection;
    private Vector3 throwHeadDirection;
    private Vector3 bombDirection;
    private RaycastHit slopeHit;

    private Rigidbody rb;
    private Animator anim;
    private GameObject model;
    private GameObject hitbox;
    private GameObject spawnPoint;
    
    [HideInInspector] public bool canInteract = false;
    [HideInInspector] public GameObject currentInteractable;
    [HideInInspector] public bool isBombJumping = false;
    [HideInInspector] public Vector3 bombForce = new Vector3(0,0,0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        model = transform.Find("Model").gameObject;
        anim = model.GetComponent<Animator>();
        hitbox = transform.Find("Hitbox").gameObject;
        
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        aimAction = InputSystem.actions.FindAction("Aim");
        throwAction = InputSystem.actions.FindAction("Attack");
        interactAction = InputSystem.actions.FindAction("Interact");

        lookDirection = model.transform.forward;

        // Spawn on Spawn Point
        spawnPoint = playerSpawner.transform.Find("SpawnPoint").gameObject;
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position + new Vector3(0,playerHeight*.5f,0), Vector3.down, playerHeight * .5f + groundDistance, groundLayer);

        if (!isSpawning)
        {
            moveValue = moveAction.ReadValue<Vector2>();

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

            if (!isInteracting)
            {
                if (moveAction.IsPressed() && isGrounded && !anim.GetCurrentAnimatorStateInfo(0).IsName("Running") && !anim.GetNextAnimatorStateInfo(0).IsName("Running") && !jumpAction.IsPressed())
                {
                    anim.CrossFadeInFixedTime("Running", .1f, 0);
                }
                if (moveAction.WasReleasedThisFrame() && isGrounded)
                {
                    anim.CrossFadeInFixedTime("Idle", .1f, 0);
                }
                
                if (jumpAction.IsPressed() && isGrounded && !isInAir && !isJumping && !isFalling)
                {
                    isInAir = true;
                    isJumping = true;
                    anim.CrossFadeInFixedTime("Jump_Start", .1f, 0);
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
                
                if (aimAction.WasPressedThisFrame() && !isAiming && !isThrowing)
                {
                    isAiming = true;
                    anim.SetFloat("ThrowSpeed", 1);
                }
                else if (aimAction.WasReleasedThisFrame() && isAiming && !isThrowing)
                {
                    isAiming = false;
                    anim.SetFloat("ThrowSpeed", -1);
                }
                
                if (throwAction.WasPressedThisFrame() && isAiming)
                {
                    isThrowing = true;
                    anim.SetFloat("ThrowSpeed", 1);
                }
                
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Running") && !isRunning && !anim.GetNextAnimatorStateInfo(0).IsName("Idle"))
                {
                    anim.CrossFadeInFixedTime("Idle", .1f, 0); 
                }
            }

            // Falling off ledge
            if (rb.linearVelocity.y < -5f && !isJumping && !isFalling && !isInAir && !OnSlope() && !isGrounded)
            {
                isInAir = true;
                isFalling = true;
                anim.CrossFadeInFixedTime("Jump_Idle", .1f, 0);
            }

            // On player landing
            if (isGrounded && !lastGrounded)
            {
                isInAir = false;
                isJumping = false;
                isFalling = false;
                isBombJumping = false;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
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

            if (interactAction.WasPressedThisFrame() && canInteract && isGrounded && !isAiming && !isThrowing && !isInteracting)
            {
                isInteracting = true;
                rb.linearVelocity = new Vector3(0,0,0);
                model.transform.LookAt(new Vector3(currentInteractable.transform.position.x, model.transform.position.y, currentInteractable.transform.position.z));
                anim.CrossFadeInFixedTime("Interact", .1f, 0);
                currentInteractable.GetComponent<LeverTrigger>().Trigger();
            }
        }

        // Store previous Grounded state
        lastGrounded = isGrounded;
    }
    
    void FixedUpdate()
    {
        if (!isInteracting)
        {
            if (OnSlope())
            {
                rb.useGravity = false;
                rb.linearVelocity = GetSlopeMoveDirection() * speed * Time.deltaTime;
            }
            else if (isBombJumping)
            {
                rb.useGravity = true;
                rb.linearVelocity = new Vector3(Mathf.MoveTowards(rb.linearVelocity.x, 0, 10f * Time.deltaTime), rb.linearVelocity.y, Mathf.MoveTowards(rb.linearVelocity.z, 0, 10f * Time.deltaTime));
            }
            else
            {
                rb.useGravity = true;
                // print(rb.linearVelocity);
                // bombForce = new Vector3(Mathf.MoveTowards(bombForce.x, 0, 10f * Time.deltaTime), Mathf.MoveTowards(bombForce.y, 0, 10f * Time.deltaTime), Mathf.MoveTowards(bombForce.z, 0, 10f * Time.deltaTime));
                // Vector3.MoveTowards(bombForce, new Vector3(0,0,0), 10f * Time.deltaTime);
                rb.linearVelocity = new Vector3(moveValue.x * speed * Time.deltaTime, rb.linearVelocity.y, moveValue.y * speed * Time.deltaTime);
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * jumpMult * Time.deltaTime;
        }
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * jumpMult * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
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
                    head.transform.LookAt(throwHeadDirection);
                }
            }
            else
            {
                head.transform.LookAt(throwHeadDirection);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // int otherObjectLayerMask = 1 << other.gameObject.layer; 

        // // Use the bitwise AND operator to check for overlap
        // if ((groundLayer.value & otherObjectLayerMask) != 0) 
        // {
        //     print(other+" entered");
        //     isGrounded = true;
        //     isFalling = false;
        //     if (groundCount <= 0)
        //     {
        //         if (isRunning)
        //         {
        //             if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        //             {
        //                 anim.CrossFadeInFixedTime("Running", .1f, 0); 
        //             }
        //         }
        //         else
        //         {
        //             anim.CrossFadeInFixedTime("Jump_Land", .1f, 0);
        //         }
        //     }
        //     groundCount += 1;
        // }
        // else
        // {

        // }
    }

    void OnTriggerExit(Collider other)
    {
        // int otherObjectLayerMask = 1 << other.gameObject.layer; 

        // // Use the bitwise AND operator to check for overlap
        // if ((groundLayer.value & otherObjectLayerMask) != 0) 
        // {
        //     groundCount -= 1;
        //     print(other+" exited");
        //     if (groundCount <= 0)
        //     {
        //         isGrounded = false;
        //         if (!anim.GetNextAnimatorStateInfo(0).IsName("Jump_Start"))
        //         {
        //             // anim.CrossFadeInFixedTime("Jump_Idle", .1f, 0);
        //         }
        //     }
        // }
        // else
        // {

        // }
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

            if (aimAction.IsPressed())
            {
                isAiming = true;
                anim.SetFloat("ThrowSpeed", 1);
            }
            else
            {
                isAiming = false;
            }
        }
    }
    

    public void ThrowBomb()
    {
        GameObject bomb = Instantiate(bombPrefab, bombSpawn.transform.position, bombSpawn.transform.rotation);
        bomb.GetComponent<Rigidbody>().AddForce(bombDirection * 20f, ForceMode.Impulse);
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


    private void SpawnPlayer()
    {
        model.SetActive(true);
        anim.CrossFadeInFixedTime("Spawn_Ground", .1f, 0);
        transform.position = spawnPoint.transform.position;
    }

    public void ResetBombJump()
    {
        isBombJumping = false;
    }

    public void Die()
    {
        
    }
}

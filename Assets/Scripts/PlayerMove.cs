using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public GameObject cam;
    public GameObject head;
    public GameObject bombSpawn;
    public GameObject bombPrefab;

    private InputAction moveAction;
    private Vector2 moveValue;
    private InputAction jumpAction;
    private InputAction aimAction;
    private InputAction throwAction;
    private InputAction interactAction;

    private float speed = 300f;
    private float jumpForce = 12f;
    private float jumpMult = 2f;
    private bool isGrounded = false;
    private bool isRunning = false;
    private bool isAiming = false;
    private bool isThrowing = false;
    private bool isInteracting = false;
    private Vector3 lookDirection;
    private int groundCount = 0;
    private Vector3 throwHeadDirection;
    private Vector3 bombDirection;

    private Rigidbody rb;
    private Animator anim;
    private GameObject model;
    private GameObject hitbox;
    
    [HideInInspector] public bool canInteract = false;
    public GameObject currentInteractable;

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
    }

    // Update is called once per frame
    void Update()
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
            if (moveAction.WasPressedThisFrame() && isGrounded)
            {
                anim.CrossFadeInFixedTime("Running", .1f, 0);
            }
            else if (moveAction.WasReleasedThisFrame() && isGrounded)
            {
                anim.CrossFadeInFixedTime("Idle", .1f, 0);
            }
            
            if (jumpAction.WasPressedThisFrame() && isGrounded)
            {
                anim.CrossFadeInFixedTime("Jump_Start", .1f, 0);
                isGrounded = false;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }

            // if (aimAction.IsPressed() && !isThrowing)
            // {
            // }
            // else if (!isThrowing)
            // {
            // }
            
            if (aimAction.WasPressedThisFrame() && !isAiming && !isThrowing)
            {
                print("asdd");
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
    
    void FixedUpdate()
    {
        if (!isInteracting)
        {
            rb.linearVelocity = new Vector3(moveValue.x * speed * Time.deltaTime, rb.linearVelocity.y, moveValue.y * speed * Time.deltaTime);
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
                
                if (Physics.Raycast(ray, out hit)) 
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
        isGrounded = true;
        if (groundCount <= 0)
        {
            if (isRunning)
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
        groundCount += 1;
    }

    void OnTriggerExit(Collider other)
    {
        groundCount -= 1;
        if (groundCount <= 0)
        {
            isGrounded = false;
            if (!anim.GetNextAnimatorStateInfo(0).IsName("Jump_Start"))
            {
                anim.CrossFadeInFixedTime("Jump_Idle", .1f, 0);
            }
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
}

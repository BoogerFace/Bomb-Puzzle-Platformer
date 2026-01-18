using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public GameObject cam;

    private InputAction moveAction;
    private Vector2 moveValue;
    private InputAction jumpAction;
    private InputAction aimAction;
    private InputAction atkAction;

    private float speed = 300f;
    private float jumpForce = 12f;
    private float jumpMult = 2f;
    private bool isGrounded = true;
    private bool isAiming = false;

    private Rigidbody rb;
    private Animator anim;
    private GameObject model;
    private GameObject hitbox;

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
        atkAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (moveAction.WasPerformedThisFrame() && !isAiming)
        {
            model.transform.LookAt(new Vector3(model.transform.position.x + moveValue.x, model.transform.position.y, model.transform.position.z + moveValue.y));
        }

        if (moveAction.IsPressed())
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        cam.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        if (jumpAction.WasPressedThisFrame() && isGrounded && !isAiming)
        {
            isGrounded = false;
            anim.SetBool("isJumping", true);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        if (rb.linearVelocity.y < -1)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }

        if (aimAction.WasPerformedThisFrame() && isGrounded && !isAiming)
        {
            isAiming = true;
            rb.linearVelocity = new Vector3(0,0,0);
            anim.SetBool("isAiming", true);
            anim.SetBool("isRunning", false);
        }
        else if (aimAction.WasCompletedThisFrame())
        {
            anim.SetBool("isAiming", false);
        }

        if (aimAction.IsPressed() && isGrounded && anim.GetBool("isAiming"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                
                // Do something with the object that was hit by the raycast.
                // GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // capsule.transform.position = hit.point;
                // Destroy(capsule.GetComponent<SphereCollider>());
                // Destroy(capsule, .1f);

                model.transform.LookAt(new Vector3(hit.point.x, model.transform.position.y, hit.point.z));
            }

            if (atkAction.WasPerformedThisFrame())
            {
                anim.SetBool("isAiming", false);
                anim.SetBool("isThrowing", true);
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!isAiming)
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

    void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
        anim.SetBool("isJumping", false);
        anim.SetBool("isFalling", false);   
    }

    void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }

    public void EndAim()
    {
        isAiming = false;
        model.transform.LookAt(new Vector3(model.transform.position.x + moveValue.x, model.transform.position.y, model.transform.position.z + moveValue.y));
    }

    public void EndThrow()
    {
        isAiming = false;
        anim.SetBool("isThrowing", false);
        model.transform.LookAt(new Vector3(model.transform.position.x + moveValue.x, model.transform.position.y, model.transform.position.z + moveValue.y));
    }
}

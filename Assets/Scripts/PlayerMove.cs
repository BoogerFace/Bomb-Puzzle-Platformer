using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public GameObject cam;

    private InputAction moveAction;
    private Vector2 moveValue;
    private InputAction jumpAction;
    private InputAction aimAction;

    private float speed = 300f;
    private float jumpForce = 10f;
    private float jumpMult = 2f;
    private bool isGrounded = false;
    private bool isAiming = false;

    private Rigidbody rb;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        aimAction = InputSystem.actions.FindAction("Aim");
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (moveAction.WasPerformedThisFrame())
        {
            transform.LookAt(new Vector3(transform.position.x + moveValue.x, transform.position.y, transform.position.z + moveValue.y));
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
        
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            isGrounded = false;
            anim.SetBool("isJumping", true);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        if (rb.linearVelocity.y < -10)
        {
            anim.SetBool("isFalling", true);
        }

        if (aimAction.WasPerformedThisFrame())
        {
            // isAiming = true;
            // anim.SetBool("isAiming", true);
        }
        else if (aimAction.WasCompletedThisFrame())
        {
            // isAiming = false;
            // anim.SetBool("isAiming", false);
        }
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveValue.x * speed * Time.deltaTime, rb.linearVelocity.y, moveValue.y * speed * Time.deltaTime);

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
        // anim.SetBool("isJumping", true);
    }
}

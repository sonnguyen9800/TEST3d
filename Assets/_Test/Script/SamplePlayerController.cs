using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Test.Script
{
    public class SamplePlayerController : MonoBehaviour
    {
    // Ground Movement
    [SerializeField]
    private Rigidbody rb;
    public float MoveSpeed = 5f;
    private float moveHorizontal;
    private float moveForward;

    // Jumping
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f; // Multiplies gravity when falling down
    public float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump
    
    [SerializeField]
    private bool isGrounded = true;
    
    public LayerMask groundLayer;
    [SerializeField]
    private float groundCheckTimer = 0f;
    [SerializeField]
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    [SerializeField]
    private float raycastDistance;

    private Vector2 _moveInput;

    private void OnCollisionEnter(Collision other)
    {
    
        // Check if the collided layer is part of the target layer mask
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Debug.LogError("On Ground");
            isGrounded = true;
            
        }
    }

    
    private void OnCollisionExit(Collision other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Debug.LogError("On Air");
            isGrounded = false;
            
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Set the raycast to be slightly beneath the player's feet
        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
        // HandleMovement(res.x, res.y);
    }


    
    void Update()
    {
        // moveHorizontal = Input.GetAxisRaw("Horizontal");
        // moveForward = Input.GetAxisRaw("Vertical");

       // RotateCamera();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Checking when we're on the ground and keeping track of our ground check delay
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            //isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
            Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red);

        }
        else
        {
            groundCheckTimer -= Time.deltaTime;
        }

    }

    private void OnDrawGizmos()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red);

    }

    void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpPhysics();
    }

    void MovePlayer()
    {

        moveHorizontal = _moveInput.x;
        moveForward = _moveInput.y;
        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 targetVelocity = movement * MoveSpeed;

        // Apply movement to the Rigidbody
        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        // If we aren't moving and are on the ground, stop velocity so we don't slide
        if (isGrounded && moveHorizontal == 0 && moveForward == 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }


    void Jump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Initial burst for the jump
    }

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0) 
        {
            // Falling: Apply fall multiplier to make descent faster
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        } // Rising
        else if (rb.linearVelocity.y > 0)
        {
            // Rising: Change multiplier to make player reach peak of jump faster
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier  * Time.fixedDeltaTime;
        }
    }
    }
}
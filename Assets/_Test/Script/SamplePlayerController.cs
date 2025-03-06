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

    [SerializeField] private Transform _targetLookUp;
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

    [SerializeField] private RotateMesh _meshRotate = null;
    private Vector2 _moveInput;

    private void OnCollisionEnter(Collision other)
    {

        // Check if the collided layer is part of the target layer mask
        if (((1 << other.gameObject.layer) & groundLayer) == 0) return;
        Debug.LogError("On Ground");
        isGrounded = true;
    }

    
    private void OnCollisionExit(Collision other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) == 0) return;
        Debug.LogError("On Air");
        isGrounded = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.inertiaTensor = rb.inertiaTensor;
        rb.inertiaTensorRotation = rb.inertiaTensorRotation;
        // Set the raycast to be slightly beneath the player's feet
        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
        // HandleMovement(res.x, res.y);
    }

    public void OnJumpTrigger()
    {
        if (isGrounded)
            Jump();
    }
    
    private void OnDrawGizmos()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red);
        Vector3 direction = _targetLookUp.transform.position - transform.position;
        Debug.DrawRay(transform.position, direction, Color.green);
        //Debug.DrawLine(_targetLookUp.transform.position, transform.position);

    }

    void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpPhysics();
        //RotatePlayer();

    }

    
    
    private void RotatePlayer()
    {
        
        Vector3 direction = _targetLookUp.transform.position - transform.position;
        
        transform.rotation = Quaternion.identity;
        direction.y = 0;
        float xAngle = Vector3.Angle(transform.forward, direction);
        if (direction.x < 0)
            xAngle *= -1;
        //transform.rotation = Quaternion.Euler(0, xAngle, 0);
        rb.MoveRotation(Quaternion.Euler(0, xAngle, 0));
//         headBone.rotation = Quaternon.identity; //Just to simplify calculation lets remove the current rotation for now
//
// //Get the vector from your head to the opponent
//         Vector3 toOpponent = opponent - headBone.position;
//         toOpponent.x = 0; //Put the vector in the YZ plane so that we get a simple x rotation. This is ultimately the direction we want to look in.
//
//         float xAngle = Vector3.Angle(headbone.forward, toOpponent);
// //Returns the shortest angle (between 0 and 180), so we need to normalize from -180 to 180
//         if(toOpponent.y < 0) xAngle *= -1;
//
//         headBone.rotation = Quaternion.Euler(xAngle, 0, 0);


    }

    public void OnMouseDown()
    {

    }


    private void RotatePlayerByMouse()
    {
        var mousePos = Input.mousePosition;
        Debug.LogError("Screen pos" + mousePos);
        var globalMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Debug.LogError("Global pos:" + globalMousePos);
        Vector3 direction = globalMousePos - transform.position;
        Debug.LogError("Direction " + direction);

        transform.rotation = Quaternion.identity;
        direction.y = 0;
        float xAngle = Vector3.Angle(transform.forward, direction);
        if (direction.x < 0)
            xAngle *= -1;
        //transform.rotation = Quaternion.Euler(0, xAngle, 0);
        rb.MoveRotation(Quaternion.Euler(0, xAngle, 0));
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
           // rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        
        var targetVector = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * movement;
        _meshRotate.Rotate(targetVector);

       // RotateTowardMovementVector(targetVector);

    }
    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if(movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 5.0f);
        //rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, 5.0f));
        
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
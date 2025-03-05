using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterControllerDemo : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController = null;
    private Camera _camera = null;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _rotationSpeed = 2.0f;
    
    [Header("Jump Settings")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    
    
    private Vector2 _moveInput;
    private Vector2 _mouseDelta;
    private void Awake()
    {
        _camera = Camera.main;
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        // HandleMovement(res.x, res.y);
    }

    public void Jump()
    {
        Debug.Log("Jump");
    }

    private void Update()
    {
        HandleMovement();
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        
    }
    
    void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _characterController.Move(move * (_moveSpeed * Time.deltaTime));
    }

}

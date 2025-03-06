using System;
using System.Collections;
using System.Text;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Test.Script
{
    public class SamplePlayerController : NetworkBehaviour, IAfterSpawned, IPlayerLeft
    {
        // Ground Movement
        [SerializeField] private Rigidbody rb;
        public float MoveSpeed = 5f;
        private float moveHorizontal;
        private float moveForward;

        // Jumping
        public float jumpForce = 10f;
        public float fallMultiplier = 2.5f; // Multiplies gravity when falling down
        public float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump

        [SerializeField] private bool isGrounded = true;

        public LayerMask groundLayer;
        [SerializeField] private float groundCheckTimer = 0f;
        [SerializeField] private float groundCheckDelay = 0.3f;
        [SerializeField] private float raycastDistance;

        [SerializeField] private RotateMesh _meshRotate = null;
        private Vector2 _moveInput;

        [SerializeField] float _sprintSpeed = 6f;
        [SerializeField] private float _sprintDuration = 0.4f; // Sprint lasts for 2 seconds
        [SerializeField] private float _sprintCooldown = 1.5f; // Cooldown before sprinting again

        private bool isSprinting = false;
        private float sprintEndTime = 0f;
        private float nextSprintTime = 0f;

        private bool _lockMovement = false;
        private float playerHeight;

        [SerializeField] private MeshColorChanger _meshColorChanger = null;
        [FormerlySerializedAs("_nickNameChanger")] [SerializeField] private NickNameController nickNameController = null;

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


        public void UpdateColor()
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            _meshColorChanger.MeshColor = randomColor;
        }

        
        string GenerateRandomName(int minLength, int maxLength)
        {
            string consonants = "bcdfghjklmnpqrstvwxyz";
            string vowels = "aeiou";

            int length = Random.Range(minLength, maxLength + 1);
            StringBuilder name = new StringBuilder();

            // Start with an uppercase consonant
            name.Append(char.ToUpper(consonants[Random.Range(0, consonants.Length)]));

            for (int i = 1; i < length; i++)
            {
                if (i % 2 == 0)
                    name.Append(consonants[Random.Range(0, consonants.Length)]);
                else
                    name.Append(vowels[Random.Range(0, vowels.Length)]);
            }

            return name.ToString();
        }
        public void UpdateName()
        {
            nickNameController.Name = GenerateRandomName(3, 8);
        }
        public void UpdateName(string value)
        {
            nickNameController.Name = value;
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
        }

        void FixedUpdate()
        {
            return;
            MovePlayer();
            ApplyJumpPhysics();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            MovePlayer();
            ApplyJumpPhysics();
            InputSystem.Update();
        }


        void MovePlayer()
        {
            if (_lockMovement)
                return;
            if (_moveInput.magnitude == 0)
                return;
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
        }

        public void Sprint()
        {
            Debug.LogError("Sprinting");
            if (Time.time <= nextSprintTime)
                return;
            if (!isGrounded)
                return;
            StartCoroutine(OnSprint());
        }


        private IEnumerator OnSprint()
        {
            isSprinting = true;
            float startTime = Time.time;
            sprintEndTime = startTime + _sprintDuration;

            while (Time.time < sprintEndTime)
            {
                Vector3 direction = rb.linearVelocity;
                rb.AddForce(direction * (_sprintSpeed - MoveSpeed), ForceMode.VelocityChange);
                yield return null;
            }

            isSprinting = false;
            nextSprintTime = Time.time + _sprintCooldown; // Enforce cooldown
        }

        void Jump()
        {
            isGrounded = false;
            groundCheckTimer = groundCheckDelay;
            rb.linearVelocity =
                new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Initial burst for the jump
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
                rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime;
            }
        }

        public static SamplePlayerController LocalPlayer;
        public void AfterSpawned()
        {
            if (Object.HasInputAuthority)
            {
                LocalPlayer = this;
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (Object.HasInputAuthority)
            {
                LocalPlayer = null;
            }
        }

        public void LockMovement(bool isLocked)
        {
            _lockMovement = isLocked;
        }
    }
}
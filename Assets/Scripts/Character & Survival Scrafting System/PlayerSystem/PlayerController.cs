using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class PlayerController : MonoBehaviour
    {
        [Space]
        [Header("Player Controller")]
        [Space]

        [SerializeField] private float jumpForce = 50f;

        [Space]

        [Space]
        [Header("Camera System")]
        [Space]

        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;

        [Space]

        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 1f;

        [Space]
        [Header("Player Settings")]
        [Space]

        [SerializeField] private float DistanceGround = 0.8f;

        [Space]

        [SerializeField] private LayerMask GroundCheck;

        [Space]

        public static PlayerController instance;
        private float AnimBlendSpeed = 12f;
        private Rigidbody rig;
        private InputManager inputManager;
        private Animator animator;
        private bool grounded = false;
        private bool hasAnimator;
        private int xVelocity;
        private int yVelocity;
        private int zVelocity;
        private int jumping;
        private int grounding;
        private float xRotation;
        private const float walkSpeed = 3.5f;
        private const float runSpeed = 5.5f;
        private Vector2 currentVelocity;
        private Vector2 currentStaticVelocity;
        [HideInInspector] public bool cursor = true;

        private bool isAttacking = false;

        private void Awake()
        {
            instance = this;
        }

        public void ToggleCursor(bool toggle)
        {
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            cursor = !toggle;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            hasAnimator = TryGetComponent<Animator>(out animator);
            rig = GetComponent<Rigidbody>();
            inputManager = GetComponent<InputManager>();

            xVelocity = Animator.StringToHash("X_Velocity");
            yVelocity = Animator.StringToHash("Y_Velocity");
            zVelocity = Animator.StringToHash("Z_Velocity");
            jumping = Animator.StringToHash("Jump");
            grounding = Animator.StringToHash("Grounded");
        }

        private void FixedUpdate()
        {
            Grounding();
            Movement();
            Jumping();
        }

        private void LateUpdate()
        {
            if (cursor == true)
            {

                    CamMovements();
            }
        }

        private void Movement()
        {
            if (!hasAnimator) return;

            float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;
            if (inputManager.Move == Vector2.zero) targetSpeed = 0;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            var xVelocityDifference = currentVelocity.x - rig.velocity.x;
            var zVelocityDifference = currentVelocity.y - rig.velocity.z;

            rig.AddForce(transform.TransformVector(new Vector3(xVelocityDifference, 0, zVelocityDifference)), ForceMode.VelocityChange);

            animator.SetFloat(xVelocity, currentVelocity.x);
            animator.SetFloat(yVelocity, currentVelocity.y);
        }

        private void CamMovements()
        {
            if (!hasAnimator) return;

            var MouseX = inputManager.Look.x;
            var MouseY = inputManager.Look.y;
            if (!isAttacking)
            {
                Camera.position = CameraRoot.position;
            }

            xRotation -= MouseY * MouseSensitivity * Time.smoothDeltaTime;
            xRotation = Mathf.Clamp(xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
            rig.MoveRotation(rig.rotation * Quaternion.Euler(0, MouseX * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void Jumping()
        {
            if (!inputManager.Jump) return;

            if (!grounded) return;

            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }

        private void Grounding()
        {
            RaycastHit hitInfo;
            Vector3 rayOrigin = rig.worldCenterOfMass;
            Vector3 rayDirection = Vector3.down;
            float rayLength = DistanceGround + 0.1f;

            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayLength, GroundCheck))
            {
                grounded = true;
                return;
            }

            grounded = false;
            animator.SetFloat(zVelocity, rig.velocity.y);
        }

        private void SetAnimationGrounding()
        {
            animator.ResetTrigger(jumping);
            animator.SetBool(grounding, grounded);
        }

        // 检测是否在攻击动画中
        public void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }
    }

}

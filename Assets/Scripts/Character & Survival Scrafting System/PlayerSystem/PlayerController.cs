using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LPSurvivalEngine
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance { get; private set; }

        [Header("Player Controller")]
        [SerializeField] private float jumpForce = 50f;

        [Header("Camera System")]
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;

        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 1f;

        [Header("Player Settings")]
        [SerializeField] private float DistanceGround = 0.8f;
        [SerializeField] private LayerMask GroundCheck;

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

        // 新增：用于记录下蹲状态
        private bool isCrouching = false;
        // 新增：下蹲时摄像机下移的距离
        private float crouchCameraOffset = 0.5f;
        // 新增：下蹲时的移动速度系数，这里设置为0.5表示速度变慢一倍
        private float crouchSpeedModifier = 0.5f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
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
            // 新增：处理下蹲逻辑
            Crouching();
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

            // 新增：根据下蹲状态调整目标速度
            float targetSpeed = isCrouching ? runSpeed * crouchSpeedModifier : (inputManager.Run ? runSpeed : walkSpeed);
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
                // 新增：根据下蹲状态调整摄像机位置
                Camera.position = CameraRoot.position + (isCrouching ? Vector3.down * crouchCameraOffset : Vector3.zero);
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

        // 判断是否正在攻击
        public void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        // 新增：处理下蹲的方法
        private void Crouching()
        {
            if (inputManager.Crouch)
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }
    }
}
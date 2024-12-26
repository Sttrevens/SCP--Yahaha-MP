using Fusion;
using LPSurvivalEngine;
using UnityEngine;

public class PlayerMovement2 : NetworkBehaviour
{
    [Header("Player Controller")]
    [SerializeField] private float jumpForce = 50f;

    [Header("Camera System")]
    // [SerializeField] private Transform CameraRoot;
    [SerializeField] private Camera Camera;

    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottomLimit = 70f;
    [SerializeField] private float MouseSensitivity = 1f;

    [Header("Player Settings")]
    [SerializeField] private float DistanceGround = 0.8f;
    [SerializeField] private LayerMask GroundCheck;

    private float AnimBlendSpeed = 12f;
    // 移除原来的刚体相关定义
    // private Rigidbody rig;
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

    private CharacterController _controller;

    private void Awake()
    {
        // 获取CharacterController组件
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = transform;

            Cursor.lockState = CursorLockMode.Locked;

            hasAnimator = TryGetComponent<Animator>(out animator);
            // 移除原来获取刚体的代码
            // rig = GetComponent<Rigidbody>();
            inputManager = FindObjectOfType<InputManager>().GetComponent<InputManager>();

            xVelocity = Animator.StringToHash("X_Velocity");
            yVelocity = Animator.StringToHash("Y_Velocity");
            zVelocity = Animator.StringToHash("Z_Velocity");
            jumping = Animator.StringToHash("Jump");
            grounding = Animator.StringToHash("Grounded");
        }
    }
    

    public override void FixedUpdateNetwork()
    {
        Grounding();
        // 替换移动逻辑
        MovementWithCharacterController();
        Jumping();
        // 新增：处理下蹲逻辑
        Crouching();

        if (cursor == true)
        {
            CamMovements();
        }
    }

    // 新的基于CharacterController的移动方法
    private void MovementWithCharacterController()
    {
        if (!hasAnimator) return;

        // 新增：根据下蹲状态调整目标速度
        float targetSpeed = isCrouching ? runSpeed * crouchSpeedModifier : (inputManager.Run ? runSpeed : walkSpeed);
        if (inputManager.Move == Vector2.zero) targetSpeed = 0;

        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(inputManager.Move.x, 0, inputManager.Move.y) * targetSpeed;

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        _velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        if (inputManager.Jump && _controller.isGrounded)
        {
            _velocity.y += jumpForce;
        }

        _controller.Move(move * Time.fixedDeltaTime + _velocity * Time.fixedDeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        animator.SetFloat(xVelocity, move.x);
        animator.SetFloat(yVelocity, move.z);
    }

    private void CamMovements()
    {
        if (!hasAnimator) return;

        var MouseX = inputManager.Look.x;
        var MouseY = inputManager.Look.y;
        if (!isAttacking)
        {
            // 新增：根据下蹲状态调整摄像机位置
            Camera.transform.position = transform.position + (isCrouching ? Vector3.down * crouchCameraOffset : Vector3.zero);
        }

        xRotation -= MouseY * MouseSensitivity * Time.smoothDeltaTime;
        xRotation = Mathf.Clamp(xRotation, UpperLimit, BottomLimit);

        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, MouseX * MouseSensitivity * Time.deltaTime, 0);
    }

    private void Jumping()
    {
        // 这里的跳跃逻辑已经整合到MovementWithCharacterController方法中了，可移除原有的跳跃逻辑判断（也可以保留，看具体需求）
        // if (!inputManager.Jump) return;
        //
        // if (!grounded) return;
        //
        // rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        // grounded = false;
    }

    private void Grounding()
    {
        RaycastHit hitInfo;
        Vector3 rayOrigin = _controller.bounds.center;
        Vector3 rayDirection = Vector3.down;
        float rayLength = DistanceGround + 0.1f;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayLength, GroundCheck))
        {
            grounded = true;
            return;
        }

        grounded = false;
        animator.SetFloat(zVelocity, _controller.velocity.y);
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

    // 定义一个用于存储角色速度的变量，类似原来刚体中的速度概念
    private Vector3 _velocity;
}
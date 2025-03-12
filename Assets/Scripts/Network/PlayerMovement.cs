using System;
using Fusion;
using UnityEngine;
using LPSurvivalEngine;
using UnityEngine.Serialization;

public class PlayerMovement : NetworkBehaviour
{
    //角色视角转变动画相关的参数
    [Header("Look Animation")]
    [SerializeField] private Transform headBone;      // 头部骨骼
    [SerializeField] private Transform spineBone;     // 脊椎骨骼
    [SerializeField] private float maxLookUpAngle = 45f;    // 最大抬头角度
    [SerializeField] private float maxLookDownAngle = 45f;  // 最大低头角度
    [SerializeField] private float headRotationRatio = 0.7f;
    [SerializeField] private float spineRotationRatio = 0.3f;
    [SerializeField] private Transform[] upperBodys;
    //网络同步相关的参数
    [Networked] private float NetworkedLookAngle { get; set; }
    [Networked] public Quaternion upperBodyRotation { get; set; }
    [Header("Player Settings")]
    //角色属性(本身的属性和第一人称的属性)
    [SerializeField]private Vector3 _velocity;
    [SerializeField]private bool _jumpPressed;
    private float _targetFOV = 40f;
    private float _targetSpeed = 6f;
    private Quaternion _cameraRotationY;
    [SerializeField] private float defaultFOV = 40f;
    [SerializeField] private float sprintFOV = 60f;
    [SerializeField] private float defaultSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float fovChangeSpeed = 4f;
    [SerializeField] private float speedChangeSpeed = 4f;
    public float playerSpeed;
    public bool isMoving;
    public float jumpForce = 5f;
    public float gravityValue = -9.81f;
    // 这个变量是干嘛的 存疑Add a variable to control the rotation speed. Adjust this value according to your actual needs.
    public float rotationSpeed = 5f;
    public bool issprinting = false;
    //角色上面挂载的其他组件
    public Camera plCamera;
    private CharacterController _controller;
    private AnimatorManager _animatorManager;
    public Transform cameraRoot;
    private HealthSystem _healthSystem;
    
    //瞄准移动速度减慢相关参数
    [Header("Aim Movement")]
    public float aimSpeed = 2f;
    public bool isAiming = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animatorManager = GetComponent<AnimatorManager>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            plCamera = Camera.main;
            plCamera.GetComponent<FirstPersonCamera>().Target = cameraRoot;

            StartCoroutine(FindFirstObjectByType<GameStartEffect>().FadeFromBlack());
        }
    }

    void Update()
    {
        UpdateUpperBodyRotationLocally();
        
        if (!PlayerController.instance.cursor)
            return;

        if (HasStateAuthority && !_healthSystem.isDeadNetworked)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (_healthSystem.stamina.currentValue > 10f)
                {
                    _jumpPressed = true;
                    if (_controller.isGrounded)
                    {
                        _healthSystem.stamina.Subtract(10f);
                    }
                }
            }

            if (!isAiming)
            {
                if ((Input.GetButton("Sprint") && isMoving) && GetComponent<HealthSystem>().stamina.currentValue > 0)
                {
                    _targetFOV = sprintFOV;
                    _targetSpeed = sprintSpeed;
                    issprinting = true;
                }
                else
                {
                    _targetFOV = defaultFOV;
                    _targetSpeed = defaultSpeed;
                    issprinting = false;
                }
            }
            else
            {
                _targetFOV = defaultFOV;
                _targetSpeed = aimSpeed;
                issprinting = false;
            }
            // 插值使得更改更加平滑
            plCamera.fieldOfView =
                Mathf.Lerp(plCamera.fieldOfView, _targetFOV, fovChangeSpeed * Runner.DeltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, _targetSpeed, speedChangeSpeed * Runner.DeltaTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        Gravity();
        Move();
    }

    public void Move()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false || _healthSystem.isDeadNetworked)
        {
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }
        
        _cameraRotationY = Quaternion.Euler(0, plCamera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = Vector3.zero;
        if (PlayerController.instance.cursor)
        {
            // 优化小技巧：较小的数字不要过早的参与计算，应将小数字先相乘然后整体与大数相乘，能够减少浮点数的舍入精度误差
            move = _cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Time.fixedDeltaTime * playerSpeed);
        }
        _animatorManager.XAxis = Input.GetAxis("Horizontal");
        _animatorManager.ZAxis = Input.GetAxis("Vertical");
        _animatorManager.Speed = move.magnitude * 100f;
        
        if (_jumpPressed && _controller.isGrounded)
        {
            _animatorManager.JumpCount++;
            _velocity.y += jumpForce;
        }

        Vector3 trueMove = move + _velocity * Time.fixedDeltaTime;
        _controller.Move(trueMove);

        if (move != Vector3.zero)
        {
            // Only adjust the forward direction slightly based on the move direction when moving.
            // transform.forward = Vector3.Slerp(transform.forward, move.normalized, 0.1f);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        _jumpPressed = false;
    }

    void Gravity()
    {
        _velocity.y += gravityValue * Time.fixedDeltaTime;
    }

    void LateUpdate()
    {
        if (_healthSystem.isDeadNetworked) return;
        
        // Calculate the target rotation based on the camera's yaw rotation.
        Quaternion targetRotation = Quaternion.Euler(0, plCamera.transform.rotation.eulerAngles.y, 0);

        // Smoothly rotate the object towards the target rotation first.
        transform.rotation = targetRotation;
        //--------------------------------------控制骨骼旋转的逻辑--------------------------------------------------------------------------------
        // 本地控制的上半身旋转，基于摄像机的旋转
        if (upperBodys != null)
        {
            foreach (Transform upperBody in upperBodys)
            {
                // 本地客户端更新上半身旋转
                if (HasStateAuthority)
                {
                    //upperBody.rotation = Camera.transform.rotation;
                    upperBody.rotation = Quaternion.Lerp(upperBody.rotation, plCamera.transform.rotation, Time.fixedDeltaTime * 30f);
                    // 同步上半身旋转到服务器
                    upperBodyRotation = upperBody.rotation;
                }
                else
                {
                    // 其他客户端通过网络同步的旋转来更新
                    upperBody.rotation = upperBodyRotation;
                }
            }
        }
        if (HasStateAuthority && plCamera != null)
        {
            // 获取相机俯仰角
            float pitch = plCamera.transform.eulerAngles.x;
            if (pitch > 180) pitch -= 360;  // 转换到 -180 到 180 度范围

            // 限制角度范围
            float clampedAngle = Mathf.Clamp(pitch, -maxLookDownAngle, maxLookUpAngle);

            // 同步到网络
            NetworkedLookAngle = clampedAngle;
        }
         // 应用旋转
        if (headBone != null)
        {
            // 获取当前的欧拉角
            Vector3 currentRotation = headBone.localRotation.eulerAngles;

            // 在原有旋转基础上修改X轴的值
            headBone.localRotation = Quaternion.Euler(
                currentRotation.x,
                currentRotation.y,
                currentRotation.z+ (NetworkedLookAngle * headRotationRatio)
            );
        }

        if (spineBone != null)
        {
            Vector3 currentSpineRotation = spineBone.localRotation.eulerAngles;
            spineBone.localRotation = Quaternion.Euler(
                currentSpineRotation.x + (NetworkedLookAngle * spineRotationRatio),
                currentSpineRotation.y,
                currentSpineRotation.z
            );
        }
    }

    // 用于同步上半身旋转到网络上的函数
    private void UpdateUpperBodyRotationLocally()
    {
        // 在本地客户端，每帧将上半身的旋转传递到网络
        if (upperBodys != null && upperBodys.Length > 0)
        {
            upperBodyRotation = upperBodys[0].rotation;  // 假设第一个上半身骨骼为主
        }
    }
    // ---------------------------------------------骨骼旋转逻辑结束--------------------------------------------------

    public void BePossessed(Transform target)
    {
        transform.SetParent(target);
    }
}
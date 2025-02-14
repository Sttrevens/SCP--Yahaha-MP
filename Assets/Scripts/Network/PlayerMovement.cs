using System;
using Fusion;
using UnityEngine;
using LPSurvivalEngine;

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
    //角色属性(本身的属性和第一人称的属性)
    [SerializeField]private Vector3 _velocity;
    [SerializeField]private bool _jumpPressed;
    private float targetFOV = 40f;
    private float targetSpeed = 6f;
    [SerializeField] private float defaultFOV = 40f;
    [SerializeField] private float sprintFOV = 60f;
    [SerializeField] private float defaultSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float fovChangeSpeed = 4f;
    [SerializeField] private float speedChangeSpeed = 4f;
    public float PlayerSpeed;
    public bool isMoving;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    // 这个变量是干嘛的 存疑Add a variable to control the rotation speed. Adjust this value according to your actual needs.
    public float RotationSpeed = 5f;
    public bool issprinting = false;
    //角色上面挂载的其他组件
    public Camera Camera;
    private CharacterController _controller;
    private AnimatorManager _animatorManager;
    public Transform cameraRoot;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animatorManager = GetComponent<AnimatorManager>();

    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = cameraRoot;

            StartCoroutine(FindFirstObjectByType<GameStartEffect>().FadeFromBlack());
        }
    }

    void Update()
    {
        if (HasStateAuthority && gameObject.tag == "Player")
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (GetComponent<HealthSystem>().stamina.currentValue > 10f)
            {
                _jumpPressed = true;
                if (_controller.isGrounded)
                {
                    GetComponent<HealthSystem>().stamina.Subtract(10f);
                }
            }
        }
        if((Input.GetButton("Sprint") && isMoving) && GetComponent<HealthSystem>().stamina.currentValue > 0){
                targetFOV = sprintFOV;
                targetSpeed = sprintSpeed;
                issprinting = true;
            }
            else
            {
                targetFOV = defaultFOV;
                targetSpeed = defaultSpeed;
                issprinting = false;
            }

        Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, targetFOV, fovChangeSpeed*Time.deltaTime);
        PlayerSpeed = Mathf.Lerp(PlayerSpeed, targetSpeed, speedChangeSpeed*Time.deltaTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false && gameObject.tag != "Player")
        {
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;
        _animatorManager.Speed = move.magnitude * 100f;

        // Calculate the target rotation based on the camera's yaw rotation.
        Quaternion targetRotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

        // Smoothly rotate the object towards the target rotation first.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Runner.DeltaTime);

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _animatorManager.JumpCount++;
            _velocity.y += JumpForce;
        }

        Vector3 trueMove = move + _velocity * Runner.DeltaTime;
        _controller.Move(trueMove);

        if (move != Vector3.zero)
        {
            // Only adjust the forward direction slightly based on the move direction when moving.
            transform.forward = Vector3.Slerp(transform.forward, move.normalized, 0.1f);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        Quaternion bodyTargetRotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, bodyTargetRotation, RotationSpeed * Runner.DeltaTime);

        _jumpPressed = false;

        UpdateUpperBodyRotationLocally();
    }

    void LateUpdate()
    {
        // 本地控制的上半身旋转，基于摄像机的旋转
        if (upperBodys != null)
        {
            foreach (Transform upperBody in upperBodys)
            {
                // 本地客户端更新上半身旋转
                if (HasStateAuthority)
                {
                    //upperBody.rotation = Camera.transform.rotation;
                    upperBody.rotation = Quaternion.Lerp(upperBody.rotation, Camera.transform.rotation, Time.deltaTime * 30f);
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
        if (HasStateAuthority && Camera != null)
        {
            // 获取相机俯仰角
            float pitch = Camera.transform.eulerAngles.x;
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
                currentRotation.x + (NetworkedLookAngle * headRotationRatio),
                currentRotation.y,
                currentRotation.z
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
}
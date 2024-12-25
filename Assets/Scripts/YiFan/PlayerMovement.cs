using System;
using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;
    private Animator _animator;

    public Camera Camera;
    public Transform cameraRoot;
    public float PlayerSpeed = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    // 动画相关变量
    private int xVelocity;
    private int yVelocity;
    private const float walkSpeed = 3.5f;
    private const float runSpeed = 5.5f;
    private float AnimBlendSpeed = 12f;
    private Vector2 currentVelocity;

    private bool isCrouching = false;
    private float crouchSpeedModifier = 0.5f;
    private float crouchCameraOffset = 0.5f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // 获取Animator
        xVelocity = Animator.StringToHash("xVelocity");
        yVelocity = Animator.StringToHash("yVelocity");
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = cameraRoot;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        // 处理下蹲逻辑
        if (Input.GetKey(KeyCode.C)) // 可以根据需要修改按键
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 只同步玩家自身的移动
        if (HasStateAuthority == false)
        {
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        // 根据下蹲状态调整目标速度
        float targetSpeed = isCrouching ? runSpeed * crouchSpeedModifier : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        // 计算方向和移动
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * targetSpeed;

        // 处理跳跃
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        // 更新角色面朝方向
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // 更新动画参数
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, move.x, AnimBlendSpeed * Time.fixedDeltaTime);
        currentVelocity.y = Mathf.Lerp(currentVelocity.y, move.z, AnimBlendSpeed * Time.fixedDeltaTime);

        _animator.SetFloat(xVelocity, currentVelocity.x);
        _animator.SetFloat(yVelocity, currentVelocity.y);

        _jumpPressed = false;
    }
}

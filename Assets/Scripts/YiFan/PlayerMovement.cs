using System;
using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;
    private AnimatorManager _animatorManager;

    public Camera Camera;
    public float PlayerSpeed = 2f;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    // Add a variable to control the rotation speed. Adjust this value according to your actual needs.
    public float RotationSpeed = 5f;

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
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
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
        }

        _jumpPressed = false;
    }
}
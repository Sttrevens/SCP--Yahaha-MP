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
            Camera.GetComponent<FirstPersonCamera>().Target = transform;
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
            gameObject.transform.forward = move;
        }
 
        _jumpPressed = false;
    }
    
}
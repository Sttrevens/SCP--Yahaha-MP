using Fusion;
using UnityEngine;


public class PlayerAnimator : NetworkBehaviour
{
    [Header("Animator")]
    public Transform model;
    // PRIVATE MEMBERS
    private AnimatorManager _animatorManager;
    private Animator _animator;
    private int _lastVisibleJump;
    private int _lastVisiblePickup;
    private int _lastVisibleThrow;
    private int _lastVisibleDie;
    private int _lastVisibleDying;
    private bool _lastIsPickup;
    private bool _lastIsAiming;
    // NetworkBehaviour INTERFACE
    public override void Spawned()
    {
        _lastVisibleJump = _animatorManager.JumpCount;
        _lastVisiblePickup = _animatorManager.PickupCount;
        _lastVisibleThrow = _animatorManager.ThrowCount;
        _lastVisibleDie = _animatorManager.DieCount;
        _lastVisibleDying = _animatorManager.DyingCount;
        _lastIsPickup = _animatorManager.IsHolding;
        _lastIsAiming = _animatorManager.IsAiming;
    }

    public override void Render()
    {
        UpdateAnimations();
    }

    // MONOBEHAVIOUR
    protected void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();
        _animator = model.GetComponent<Animator>();
    }

    // PRIVATE METHODS
    private void UpdateAnimations()
    {
        if (_lastVisibleJump < _animatorManager.JumpCount)
        {
            _animator.SetTrigger("Jump");
        }
        else if (_lastVisibleJump > _animatorManager.JumpCount)
        {
            // Cancel Jump
        }
        
        if (_lastVisiblePickup < _animatorManager.PickupCount)
        {
            _animator.SetTrigger("Pickup");
        }
        else if (_lastVisiblePickup > _animatorManager.PickupCount)
        {
            // Cancel Pickup
        }
        
        if (_lastVisibleThrow < _animatorManager.ThrowCount)
        {
            _animator.SetTrigger("Throw");
        }
        else if (_lastVisibleThrow > _animatorManager.ThrowCount)
        {
            // Cancel Pickup
        }
        
        if (_lastVisibleDie < _animatorManager.DieCount)
        {
            _animator.SetTrigger("Die");
        }
        else if (_lastVisibleDie > _animatorManager.DieCount)
        {
            // Cancel Pickup
        }

        if (_lastVisibleDying < _animatorManager.DyingCount)
        {
            _animator.SetTrigger("Dying");
            
        }
        else if (_lastVisibleDying > _animatorManager.DyingCount)
        {
            //do  nothing
        }

        if (_lastIsPickup != _animatorManager.IsHolding)
        {
            Debug.Log("有反转");
            _animator.SetBool("Holding", _animatorManager.IsHolding);
        }

        if (_lastIsAiming != _animatorManager.IsAiming)
        {
            _animator.SetBool("Aiming", _animatorManager.IsAiming);
        }
        
        _lastVisibleJump = _animatorManager.JumpCount;
        _lastVisiblePickup = _animatorManager.PickupCount;
        _lastVisibleThrow = _animatorManager.ThrowCount;
        _lastVisibleDie = _animatorManager.DieCount;
        _lastVisibleDying = _animatorManager.DyingCount;
        _lastIsPickup = _animatorManager.IsHolding;
        _lastIsAiming = _animatorManager.IsAiming;
        _animator.SetFloat("Speed", _animatorManager.Speed);
        _animator.SetFloat("XAxis",_animatorManager.XAxis);
        _animator.SetFloat("ZAxis", _animatorManager.ZAxis);
    }
}
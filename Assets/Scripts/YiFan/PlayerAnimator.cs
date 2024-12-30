using Fusion;
using UnityEngine;


public class PlayerAnimator : NetworkBehaviour
{
    // PRIVATE MEMBERS
    private AnimatorManager _animatorManager;
    private Animator _animator;
    private int _lastVisibleJump;
    private int _lastVisiblePickup;
    private int _lastVisibleWield;

    // NetworkBehaviour INTERFACE
    public override void Spawned()
    {
        _lastVisibleJump = _animatorManager.JumpCount;
        _lastVisiblePickup = _animatorManager.PickupCount;
        _lastVisibleWield = _animatorManager.WieldCount;
    }

    public override void Render()
    {
        UpdateAnimations();
    }

    // MONOBEHAVIOUR
    protected void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();
        _animator = GetComponentInChildren<Animator>();
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
        
        if (_lastVisibleWield < _animatorManager.WieldCount)
        {
            _animator.SetTrigger("OneHandAttack");
        }
        else if (_lastVisibleWield > _animatorManager.WieldCount)
        {
            // Cancel Pickup
        }
        
        _lastVisibleJump = _animatorManager.JumpCount;
        _lastVisiblePickup = _animatorManager.PickupCount;
        _lastVisibleWield = _animatorManager.WieldCount;
        _animator.SetFloat("Speed", _animatorManager.Speed);
    }
}
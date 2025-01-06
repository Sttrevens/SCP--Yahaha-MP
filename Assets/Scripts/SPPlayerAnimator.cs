using UnityEngine;

public class SPPlayerAnimator : MonoBehaviour
{
    // PRIVATE MEMBERS
    private SPAnimatorManager _animatorManager;
    private Animator _animator;
    private int _lastVisibleJump;
    private int _lastVisiblePickup;
    private int _lastVisibleWield;
    private int _lastVisibleTwoHandWield;

    // MONOBEHAVIOUR
    protected void Awake()
    {
        _animatorManager = GetComponent<SPAnimatorManager>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateAnimations();
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

        if (_lastVisibleTwoHandWield < _animatorManager.TwoHandWieldCount)
        {
            _animator.SetTrigger("TwoHandAttack");
        }
        else if (_lastVisibleTwoHandWield > _animatorManager.TwoHandWieldCount)
        {
            // Cancel Pickup
        }

        _lastVisibleJump = _animatorManager.JumpCount;
        _lastVisiblePickup = _animatorManager.PickupCount;
        _lastVisibleWield = _animatorManager.WieldCount;
        _lastVisibleTwoHandWield = _animatorManager.TwoHandWieldCount;
        _animator.SetFloat("Speed", _animatorManager.Speed);
    }
}
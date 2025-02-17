using Fusion;
using UnityEngine;


public class EnemyAnimator : NetworkBehaviour
{
    [Header("Animator")]
    public Transform model;
    // PRIVATE MEMBERS
    private EnemyAnimatorManager _animatorManager;
    private Animator _animator;
    private int _lastVisibleAttack;
    private int _lastVisibleChasing;
    private int _lastVisiblePatrolling;

    // NetworkBehaviour INTERFACE
    public override void Spawned()
    {
        _lastVisibleAttack = _animatorManager.AttackCount;
        _lastVisibleChasing = _animatorManager.isChasing;
        _lastVisiblePatrolling = _animatorManager.isPatrolling;
    }

    public override void Render()
    {
        UpdateAnimations();
    }

    // MONOBEHAVIOUR
    protected void Awake()
    {
        _animatorManager = GetComponent<EnemyAnimatorManager>();
        _animator = model.GetComponent<Animator>();
    }

    // PRIVATE METHODS
    private void UpdateAnimations()
    {
        if (_lastVisibleAttack < _animatorManager.AttackCount)
        {
            _animator.SetTrigger("Attack");
        }
        if (_lastVisibleChasing < _animatorManager.isChasing)
        {
            _animator.SetBool("IsChasing", true);
        }
        else
        {
            _animator.SetBool("IsChasing", false);
        }
        if (_lastVisiblePatrolling < _animatorManager.isPatrolling)
        {
            _animator.SetBool("IsPatrolling", true);
        }
        else
        {
            _animator.SetBool("IsPatrolling", false);
        }


        _lastVisibleAttack = _animatorManager.AttackCount;
        _lastVisibleChasing = _animatorManager.isChasing;
        _lastVisiblePatrolling = _animatorManager.isPatrolling;
    }
}
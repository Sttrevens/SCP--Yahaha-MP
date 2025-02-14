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

    // NetworkBehaviour INTERFACE
    public override void Spawned()
    {
        _lastVisibleAttack = _animatorManager.AttackCount;
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
        
        _lastVisibleAttack = _animatorManager.AttackCount;
    }
}
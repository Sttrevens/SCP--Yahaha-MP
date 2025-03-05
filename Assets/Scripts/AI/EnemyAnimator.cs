using Fusion;
using UnityEngine;


public class EnemyAnimator : NetworkBehaviour
{
    [Header("要挂动画Animator所在的物体，手动挂")]
    public Transform model;
    // PRIVATE MEMBERS
    private EnemyAnimatorManager _animatorManager;
    private Animator _animator;
    private int _lastVisibleAttack;
    private bool _lastVisibleChasing;
    private bool _lastVisiblePatrolling;
    private int _lastVisibleCastSpell;
    private bool _lastVisibleSpellBool;
    private bool _lastVisibleSpecialState;

    // NetworkBehaviour INTERFACE
    public override void Spawned()
    {
        _lastVisibleAttack = _animatorManager.AttackCount;
        _lastVisibleChasing = _animatorManager.isChasing;
        _lastVisiblePatrolling = _animatorManager.isPatrolling;
        _lastVisibleCastSpell = _animatorManager.CastSpellCount;
        _lastVisibleSpellBool = _animatorManager.spellingBool;
        _lastVisibleSpecialState = _animatorManager.isinSpecialState;
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
        if (_lastVisibleChasing != _animatorManager.isChasing)
        {
            _animator.SetBool("IsChasing", _animatorManager.isChasing);
        }

        if (_lastVisiblePatrolling != _animatorManager.isPatrolling)
        {
            _animator.SetBool("IsPatrolling", _animatorManager.isPatrolling);
        }

        if (_lastVisibleCastSpell < _animatorManager.CastSpellCount)
        {
            _animator.SetTrigger("Cast Spell");
        }

        if (_lastVisibleSpellBool != _animatorManager.spellingBool)
        {
            _animator.SetBool("Spell Bool", _animatorManager.spellingBool);
        }

        if (_lastVisibleSpecialState != _animatorManager.isinSpecialState)
        {
            _animator.SetBool("IsInSpecialState", _animatorManager.isinSpecialState);
        }


        _lastVisibleAttack = _animatorManager.AttackCount;
        _lastVisibleChasing = _animatorManager.isChasing;
        _lastVisiblePatrolling = _animatorManager.isPatrolling;
        _lastVisibleCastSpell = _animatorManager.CastSpellCount;
        _lastVisibleSpellBool = _animatorManager.spellingBool;
        _lastVisibleSpecialState = _animatorManager.isinSpecialState;
    }
}
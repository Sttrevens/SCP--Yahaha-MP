using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Enemy : NetworkBehaviour
{
    public float maxHealth = 100f;
    [Networked] public float currentHealth { get; set; } = 100f;
    
    [SerializeField] private bool isFlyingEnemy = false;
    
    public IEnemyState CurrentState;
    
    [SerializeField]
    public Animator animator;

    public EnemyAnimatorManager _animatorManager;
    
    [SerializeField] public List<AudioClipWithLabel> sfxClips = new List<AudioClipWithLabel>();

[Serializable]
public class AudioClipWithLabel
{
    public AudioClip clip;
    public string label;
}

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _animatorManager = GetComponent<EnemyAnimatorManager>();
    }

    public override void Spawned()
    {
        currentHealth = maxHealth;
    }
    
    public void SwitchState(IEnemyState newState)
    {
        if (CurrentState != null)
            CurrentState.ExitState(this);

        CurrentState = newState;
        CurrentState.EnterState(this);
        Debug.Log("Switching to state: " + CurrentState.GetType().Name);
    }

    public override void FixedUpdateNetwork()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this);
        }
    }

    public void TakeDamage(float damage)
    {
        
    }

    void OnDead()
    {
        
    }

    void OnDestroy()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class EyedSlimeBlueController : NetworkBehaviour
{
    private Enemy _enemy;
    private ChasingEnemy _chasingEnemy;
    
    private Animator _animator;
    
    public float tauntingRange = 2f;
    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _chasingEnemy = GetComponent<ChasingEnemy>();
        
        _animator = _enemy.animator;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
{
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, tauntingRange);
    foreach (var hitCollider in hitColliders)
    {
        if (hitCollider.CompareTag("Player") && hitCollider.gameObject == _chasingEnemy.targetPlayer)
        {
            GetComponent<NetworkMecanimAnimator>().SetTrigger("Taunt");
            return;
        }
    }
    GetComponent<NetworkMecanimAnimator>().SetTrigger("CancelTaunt");
}
}

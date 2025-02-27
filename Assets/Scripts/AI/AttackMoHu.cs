using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;

public class AttackMoHu : NetworkBehaviour
{
    public Enemy enemy;

    private ChasingEnemy _chasingEnemy;

    private EnemyAttack _enemyAttack;
    // Start is called before the first frame update
    void Start()
    {
        _chasingEnemy = enemy.GetComponent<ChasingEnemy>();
        _enemyAttack = enemy.GetComponent<EnemyAttack>();
    }

    private void MoHu()
    {
        if (_chasingEnemy.targetPlayer != null && _enemyAttack.ShouldAttackBasedOnChasingEnemy(_chasingEnemy))
        {
            HealthSystem playerHealth = _chasingEnemy.targetPlayer.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.Rpc_MoHu(5.0f);
            }
        }
    }
    
}

using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class AttackingState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        if (enemy.targetPlayer != null && enemy.targetPlayer.tag == "Player")
        {
            enemy.lastAttackTime = Time.time;
            enemy.lastAttackPreDelayTime = Time.time;
            if (enemy._animatorManager != null)
            {
                enemy._animatorManager.AttackCount++;
            }
        }
        else
        {
            enemy.SwitchState(GetComponent<ChasingState>());
        }
    }

    public override void UpdateState(EnemyAI enemy)
    {
        if (enemy.HasStateAuthority)
        {
        // Attack pre-delay before actually applying damage
        if (Time.time - enemy.lastAttackPreDelayTime >= enemy.attackPreDelay)
        {
            if (enemy.targetPlayer != null && enemy.ShouldAttack(enemy))
            {
                // Reduce player health
                HealthSystem playerHealth = enemy.targetPlayer.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.Rpc_TakePhysicDamage(enemy.attackDamage);
                    Debug.Log("Current Player health: " + playerHealth.health.currentValue + "/" + playerHealth.health.maxValue);
                }
                else { Debug.Log("Player Health is Null,"); }
            }

                enemy.SwitchState(GetComponent<WaitingforNextAttackState>());
            }
        }
    }

    public override void ExitState(EnemyAI enemy)
    {
        
    }
}
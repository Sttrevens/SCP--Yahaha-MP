using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AttackingState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        if (enemy.targetPlayer != null)
        {
            enemy.lastAttackTime = Time.time;
            enemy.lastAttackPreDelayTime = Time.time;
            enemy.animator.SetTrigger("Attack");
        }
        else
        {
            enemy.SwitchState(new ChasingState());
        }
    }

    public override void UpdateState(EnemyAI enemy)
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
                    playerHealth.TakePhysicDamage(enemy.attackDamage);
                    Debug.Log("Current Player health: " + playerHealth.health.currentValue + "/" + playerHealth.health.maxValue);
                }
                else { Debug.Log("Player Health is Null,"); }
            }

            enemy.SwitchState(new WaitingforNextAttackState());
        }
    }

    public override void ExitState(EnemyAI enemy)
    {
        
    }
}
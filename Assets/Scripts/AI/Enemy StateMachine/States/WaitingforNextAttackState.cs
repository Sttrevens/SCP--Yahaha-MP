using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WaitingforNextAttackState : EnemyBaseState
{
    public override void UpdateState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            if (Time.time - EnemyAttack.lastAttackTime >= EnemyAttack.attackInterval)
            {
                if (EnemyAttack.ShouldAttackBasedOnChasingEnemy(ChasingEnemy))
                {
                    enemy.SwitchState(new AttackingState());
                }
                else
                {
                    enemy.SwitchState(new ChasingState());
                }
            }

            // Smoothly rotate enemy to face the target player if one exists
        if (ChasingEnemy.targetPlayer != null)
        {
            Vector3 directionToTarget = ChasingEnemy.targetPlayer.transform.position - enemy.transform.position;
            directionToTarget.y = 0; // Keep rotation on the horizontal plane
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        }
    }

    public override void ExitState(Enemy enemy)
    {
       
    }
}
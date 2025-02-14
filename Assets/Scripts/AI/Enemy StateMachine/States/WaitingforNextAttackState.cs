using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WaitingforNextAttackState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
    }

    public override void UpdateState(EnemyAI enemy)
    {
        if (enemy.HasStateAuthority)
        {
            if (Time.time - enemy.lastAttackTime >= enemy.attackInterval)
            {
                if (enemy.ShouldAttack(enemy))
            {
                enemy.SwitchState(new AttackingState());
            }
            else
            {
                    enemy.SwitchState(new ChasingState());
                }
            }
        }
    }

    public override void ExitState(EnemyAI enemy)
    {
       
    }
}
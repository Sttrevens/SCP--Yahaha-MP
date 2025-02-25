using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AttackingState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        if (enemy.HasStateAuthority)
        {
            if (ChasingEnemy.targetPlayer != null)
            {
                EnemyAttack.lastAttackTime = Time.time;
                EnemyAttack.lastAttackPreDelayTime = Time.time;
                if (enemy._animatorManager != null)
                {
                    enemy._animatorManager.AttackCount++;
                }
            }
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        
    }
    

    public override void ExitState(Enemy enemy)
    {
        
    }
}
using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CastingSpellState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        if (enemy.HasStateAuthority)
        {
                if (enemy._animatorManager != null)
                {
                    enemy._animatorManager.CastSpellCount++;
                }
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            if (!EnemyAttack.ShouldAttackBasedOnChasingEnemy(ChasingEnemy))
            {
                enemy.SwitchState(new ChasingState());
            }
            else
            {
                enemy.SwitchState(new AttackingState());
            }
        }
    }
    

    public override void ExitState(Enemy enemy)
    {
        
    }
}
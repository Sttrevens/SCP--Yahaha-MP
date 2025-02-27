using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CastingSpellState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        //调动画
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
        //检测
        if (enemy.HasStateAuthority)
        {
            //检测要不要干
            if (!EnemyAttack.ShouldAttackBasedOnChasingEnemy(ChasingEnemy))
            {
                //不大就追
                enemy.SwitchState(new ChasingState());
            }
            else
            {
                //能打就打
                enemy.SwitchState(new AttackingState());
            }
        }
    }
    

    public override void ExitState(Enemy enemy)
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        enemy.Idle();
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
        if (enemy.PlayerInSight())
        {
            enemy.SwitchState(new ChasingState());
        }
    }

    public override void ExitState(EnemyAI enemy)
    {
       
    }
}

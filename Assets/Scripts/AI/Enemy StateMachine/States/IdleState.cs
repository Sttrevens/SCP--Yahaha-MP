using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        //enemy.Idle();
    }

    public override void UpdateState(Enemy enemy)
    {
        if (ChasingEnemy.PlayerInSight())
        {
            enemy.SwitchState(new ChasingState());
        }
    }

    public override void ExitState(Enemy enemy)
    {
       
    }
}

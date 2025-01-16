using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootIdleState : RootBaseState
{
    public override void EnterState(Root enemy)
    {
        //anim
    }

    public override void UpdateState(Root enemy)
    {
        print("lalal");

        if (enemy.PlayerInSight())
        {
            print("q1q1");
            enemy.SwitchState(new RootAttackState());
        }

    }

    public override void ExitState(Root enemy)
    {
       //anim
    }
}

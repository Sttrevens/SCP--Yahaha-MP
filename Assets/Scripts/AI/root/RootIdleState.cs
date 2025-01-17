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

        if (enemy.PlayerInSight())
        {
            enemy.SwitchState(new RootAttackState());
        }

    }

    public override void ExitState(Root enemy)
    {
       //anim
    }
}

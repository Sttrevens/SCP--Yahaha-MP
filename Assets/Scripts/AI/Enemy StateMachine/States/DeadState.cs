using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        enemy.animator.SetBool("IsDead", true);
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
    }

    public override void ExitState(EnemyAI enemy)
    {
        
    }
}

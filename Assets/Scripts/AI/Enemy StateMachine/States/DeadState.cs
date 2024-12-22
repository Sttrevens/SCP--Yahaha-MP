using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        if (enemy.animator != null)
            enemy.animator.SetBool("IsDead", true);
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
    }

    public override void ExitState(EnemyAI enemy)
    {
        
    }
}

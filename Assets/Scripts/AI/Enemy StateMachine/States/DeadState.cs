using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        if (enemy.animator != null)
            enemy.animator.SetBool("IsDead", true);
    }

    public override void UpdateState(Enemy enemy)
    {
        
    }

    public override void ExitState(Enemy enemy)
    {
        
    }
}

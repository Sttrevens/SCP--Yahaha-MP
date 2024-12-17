using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AttackingState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        enemy.AttackPlayer();
       
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
    }

    public override void ExitState(EnemyAI enemy)
    {
        
    }
}
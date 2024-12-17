using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChasingState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        enemy.agent.speed = enemy.chasingSpeed;
        enemy.PlayAnimation("IsChasing", true);
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
        enemy.ChasePlayer();
    }

    public override void ExitState(EnemyAI enemy)
    {
       
    }
}
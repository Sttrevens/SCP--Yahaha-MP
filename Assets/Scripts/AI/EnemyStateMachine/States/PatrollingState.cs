using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingState : EnemyBaseState
{
    private Coroutine patrolLoop;

    public override void EnterState(EnemyAI enemy)
    {
        enemy.agent.speed = enemy.patrollingSpeed;
       
        patrolLoop = enemy.StartCoroutine(PatrolLoop(enemy));
        enemy.PlayAnimation("IsPatrolling", true);
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
    }

    public override void ExitState(EnemyAI enemy)
    {
        if (patrolLoop != null)
        {
            enemy.StopCoroutine(patrolLoop);
        }
    }

    private IEnumerator PatrolLoop(EnemyAI enemy)
    {
        while (enemy.currentState is PatrollingState)
        {
            yield return enemy.StartCoroutine(enemy.Patrol());
           
            if (!(enemy.currentState is PatrollingState))
            {
                yield break;
            }
        }
    }
}

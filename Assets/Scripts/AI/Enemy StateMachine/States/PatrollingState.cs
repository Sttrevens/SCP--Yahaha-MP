using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PatrollingState : EnemyBaseState
{
    private Coroutine patrolLoop;

    public override void EnterState(EnemyAI enemy)
    {
        if (enemy.HasStateAuthority)
        {
            enemy.agent.speed = enemy.patrollingSpeed;
           
            patrolLoop = enemy.StartCoroutine(PatrolLoop(enemy));
            if (enemy._animatorManager != null)
                enemy._animatorManager.isPatrolling++;
        }
    }

    public override void UpdateState(EnemyAI enemy)
    {
        // if (enemy.agent.path.corners.Length > 1)
        // {
        //     Vector3 currentPathDirection = enemy.agent.path.corners[1] - enemy.agent.transform.position;
        //     currentPathDirection.y = 0;
        //     currentPathDirection.Normalize();

        //     enemy.RotateTowards(currentPathDirection);
        // }
    }

    public override void ExitState(EnemyAI enemy)
    {
        if (enemy.HasStateAuthority)
        {
            if (patrolLoop != null) 
            {
                enemy.StopCoroutine(patrolLoop);
            }
        }
    }

    private IEnumerator PatrolLoop(EnemyAI enemy)
    {
        while (enemy.currentState is PatrollingState)
        {
            yield return enemy.StartCoroutine(enemy.Patrol());
            Debug.Log("Patrolling AHAHAH");
           
            if (!(enemy.currentState is PatrollingState))
            {
                yield break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Fusion;

public class PatrollingState : EnemyBaseState
{
    private Coroutine patrolLoop;

    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        if (enemy.HasStateAuthority)
        { 
            EnemyMovement.agent.speed = EnemyMovement.patrollingSpeed;
           
            patrolLoop = enemy.StartCoroutine(PatrolLoop(enemy));
            if (enemy._animatorManager != null)
            {
                enemy._animatorManager.isPatrolling = true;
                enemy._animatorManager.isChasing = false;
            }
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        // if (enemy.agent.path.corners.Length > 1)
        // {
        //     Vector3 currentPathDirection = enemy.agent.path.corners[1] - enemy.agent.transform.position;
        //     currentPathDirection.y = 0;
        //     currentPathDirection.Normalize();

        //     enemy.RotateTowards(currentPathDirection);
        // }
    }

    public override void ExitState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            if (patrolLoop != null) 
            {
                enemy.StopCoroutine(patrolLoop);
            }
        }
        
        enemy._animatorManager.isPatrolling = false;
    }

    private IEnumerator PatrolLoop(Enemy enemy)
    {
        while (enemy.CurrentState is PatrollingState)
        {
            yield return EnemyMovement.StartCoroutine(EnemyMovement.Patrol());
           
            if (!(enemy.CurrentState is PatrollingState))
            {
                yield break;
            }
        }
    }
}

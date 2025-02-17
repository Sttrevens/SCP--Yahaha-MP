using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DestroyingObstacleState : EnemyBaseState
{

    public void EnterState(Enemy enemy)
    {
        EnemyAttack.lastAttackTime = Time.time;
        EnemyMovement.agent.isStopped = true;
    }

    public void UpdateState(Enemy enemy)
    {
        /*GameObject obstacle = enemy.CheckForDestroyableObstacle();
        if (obstacle != null)
        {

            if (Time.time - enemy.lastAttackTime >= enemy.attackInterval)
            {
                enemy.DestroyObstacle();
                enemy.lastAttackTime = Time.time;  
            }
        }
        else
        {
            enemy.SwitchState(new ChasingState());
        }*/
    }

    public void ExitState(Enemy enemy)
    {
        EnemyMovement.agent.isStopped = false;
    }
}
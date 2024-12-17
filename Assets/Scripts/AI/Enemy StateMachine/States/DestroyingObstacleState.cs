using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DestroyingObstacleState : IEnemyState
{

    public void EnterState(EnemyAI enemy)
    {
        enemy.lastAttackTime = Time.time;
        enemy.agent.isStopped = true;
    }

    public void UpdateState(EnemyAI enemy)
    {
        GameObject obstacle = enemy.CheckForDestroyableObstacle();
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
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.agent.isStopped = false;
    }
}
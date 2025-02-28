using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendState : EnemyBaseState
{
    public EnemyDefend defend;
    public override void EnterState(Enemy enemy)
    {
        defend = enemy.GetComponent<EnemyDefend>();
        base.EnterState(enemy);
        if (enemy.HasStateAuthority)
        {
            if (enemy._animatorManager != null) enemy._animatorManager.spellingBool = true;
            EnemyMovement.agent.speed = 0;
        }
    }
    //这个的本质也是每帧调用
    /// <summary>
    /// 进入这个状态之后应该如何处理状态切换，防御状态是在追逐过程中切换的，那只能就是切换回chasing状态
    /// </summary>
    /// <param name="enemy"></param>
    public override void UpdateState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            //检测要不要龟   当我看到了玩家，玩家也看到了我，并且两者的距离足够接近三个 条件同时满足的时候，进入龟缩
            if (ChasingEnemy.targetPlayer != null && ChasingEnemy.targetPlayer.GetComponent<EnemyInPlayerSight>().isEnemyInSight)
            {
                
            }
            else
            {
                enemy.SwitchState(new ChasingState());
            }
        }
    }

    public override void ExitState(Enemy enemy)
    {
        Debug.Log("Exit Defend State");
        enemy._animatorManager.spellingBool = false;
    }


}

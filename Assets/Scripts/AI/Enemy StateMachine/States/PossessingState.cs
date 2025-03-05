using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessingState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        enemy._animatorManager.isinSpecialState = true;
        if (enemy.GetComponent<EnemyDefend>() != null)
        {
            enemy.GetComponent<EnemyDefend>().enabled = false;
        }
        Debug.Log("我爱附体");
    }
    //这个的本质也是每帧调用
    /// <summary>
    /// 进入这个状态之后应该如何处理状态切换，防御状态是在追逐过程中切换的，那只能就是切换回chasing状态
    /// </summary>
    /// <param name="enemy"></param>
    public override void UpdateState(Enemy enemy)
    {
    }

    public override void ExitState(Enemy enemy)
    {
        Debug.Log("Exit Possessing State");
        enemy._animatorManager.isinSpecialState = false;
        if (enemy.GetComponent<EnemyDefend>() != null)
        {
            enemy.GetComponent<EnemyDefend>().enabled = true;
        }
    }
}

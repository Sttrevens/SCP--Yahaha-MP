using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeingAttackedState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        
        enemy.animator.SetTrigger("Hit");
        enemy.StartCoroutine(BeingAttackedRoutine(enemy));
    }

    private IEnumerator BeingAttackedRoutine(EnemyAI enemy)
    {
        yield return new WaitForSeconds(1f);
       
        if (enemy.health > 0)
            enemy.SwitchState(new ChasingState());
        else
            enemy.SwitchState(new DeadState());
    }

    public override void UpdateState(EnemyAI enemy)
    {
        
    }

    public override void ExitState(EnemyAI enemy)
    {
       
    }
}
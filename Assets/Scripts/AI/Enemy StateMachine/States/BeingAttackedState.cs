using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeingAttackedState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        
        if (enemy.animator != null)
            enemy.animator.SetTrigger("Hit");
        enemy.StartCoroutine(BeingAttackedRoutine(enemy));
    }

    private IEnumerator BeingAttackedRoutine(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);
       
        if (enemy.currentHealth > 0)
            enemy.SwitchState(new ChasingState());
        else
            enemy.SwitchState(new DeadState());
    }

    public override void UpdateState(Enemy enemy)
    {
        
    }

    public override void ExitState(Enemy enemy)
    {
       
    }
}
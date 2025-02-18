using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AttackingState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        if (enemy.HasStateAuthority)
        {
            if (ChasingEnemy.targetPlayer != null)
            {
                EnemyAttack.lastAttackTime = Time.time;
                EnemyAttack.lastAttackPreDelayTime = Time.time;
                if (enemy._animatorManager != null)
                {
                    enemy._animatorManager.AttackCount++;
                    // Since AttackingState is not a MonoBehaviour, we use enemy to start the coroutine.
                    enemy.StartCoroutine(Attack(enemy));
                }
            }
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        
    }
    
    private IEnumerator Attack(Enemy enemy)
    {
        PlayAttackSFX(enemy);
        
        yield return new WaitForSeconds(EnemyAttack.attackPreDelay);

        if (ChasingEnemy.targetPlayer != null && EnemyAttack.ShouldAttackBasedOnChasingEnemy(ChasingEnemy))
        {
            ReducePlayerHealth();
        }
        
        enemy.SwitchState(new WaitingforNextAttackState());
        // Use yield break to end the coroutine cleanly after switching state.
        yield break;
    }
    
    void ReducePlayerHealth()
    {
        // Reduce player health
        HealthSystem playerHealth = ChasingEnemy.targetPlayer.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.Rpc_TakePhysicDamage(EnemyAttack.attackDamage);
            Debug.Log("Current Player health: " + playerHealth.health.currentValue + "/" +
                      playerHealth.health.maxValue);
        }
        else
        {
            Debug.Log("Player Health is Null,");
        }
    }

    void PlayAttackSFX(Enemy enemy)
{
    if (enemy.sfxClips != null)
    {
        foreach (var clip in enemy.sfxClips)
        {
            if (clip.label == "Bite")
            {
                AudioManager.instance.PlaySFX(enemy.gameObject, clip.clip);
                break;
            }
        }
    }
}

    public override void ExitState(Enemy enemy)
    {
        
    }
}
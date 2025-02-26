using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

public class AttackEffect : MonoBehaviour
{
    public Enemy enemy;

    private ChasingEnemy _chasingEnemy;
    private EnemyAttack _enemyAttack;
    
    // Start is called before the first frame update
    void Start()
    {
        _chasingEnemy = enemy.GetComponent<ChasingEnemy>();
        _enemyAttack = enemy.GetComponent<EnemyAttack>();
    }

    private void Attack(int attackDamage)
    {
        PlayAttackSFX(enemy);

        if (_chasingEnemy.targetPlayer != null && _enemyAttack.ShouldAttackBasedOnChasingEnemy(_chasingEnemy))
        {
            ReducePlayerHealth(attackDamage);
        }
    }
    
    void ReducePlayerHealth(int attackDamage)
    {
        // Reduce player health
        HealthSystem playerHealth = _chasingEnemy.targetPlayer.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.Rpc_TakePhysicDamage(_enemyAttack.attackDamage + attackDamage);
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

    public void EnterCD()
    {
        enemy.SwitchState(new WaitingforNextAttackState());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

public class AttackReduceHealth : MonoBehaviour
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

    private void Attack()
    {
        PlayAttackSFX(enemy);
        // 检测到是否攻击到玩家
        if (_chasingEnemy.targetPlayer != null && _enemyAttack.ShouldAttackBasedOnChasingEnemy(_chasingEnemy))
        {
            ReducePlayerHealth();
        }
    }
    
    void ReducePlayerHealth()
    {
        // Reduce player health
        HealthSystem playerHealth = _chasingEnemy.targetPlayer.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.Rpc_TakePhysicDamage(_enemyAttack.attackDamage);
            Debug.Log("Current Player health: " + playerHealth.health.currentValue + "/" +
                      playerHealth.health.maxValue);
        }
        else
        {
            Debug.Log("Player Health is Null,");
        }
    }
    /// <summary>
    /// 播放攻击的声音
    /// </summary>
    /// <param name="enemy"></param>
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
}

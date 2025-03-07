using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
using LPSurvivalEngine;

public class EnemyAttack : NetworkBehaviour
{
    [Header("攻击相关变量")]
    public int attackDamage = 100;
    public float attackRange = 3f;
    //CD时间
    [HideInInspector] public float lastAttackTime = 0f;
    //攻击前摇（已废弃）
    [HideInInspector] public float lastAttackPreDelayTime = 0f;
    
    public float attackPreDelay = 0.5f;
    //
    public float attackInterval = 3f;
    
    [HideInInspector] public Enemy enemy;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }
    
    /// <summary>
    /// 检测逻辑实现原理：首先求得眼球处朝向面前的射线，然后偏移4个角度，分别投射射线检测有没有标签是“player”的玩家
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns>是不是有玩家被检测到</returns>
    public bool ShouldAttackBasedOnChasingEnemy(ChasingEnemy enemy)
    {
        // First check if target player exists and is within attack range
        if (enemy.targetPlayer != null)
        {
            if (enemy.targetPlayer.GetComponent<HealthSystem>().isDeadNetworked)
            {
                return false;
            }
            
            //计算与要攻击的玩家的距离
            float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);
            if (distanceToTarget <= attackRange)
            {
                //检测逻辑实现原理：首先求得眼球处朝向面前的射线，然后偏移4个角度，分别投射射线检测有没有标签是“player”的玩家
                // Check if there are obstacles between enemy and player
                //Vector3 directionToPlayer = (enemy.targetPlayer.transform.position - enemy.transform.position).normalized;
                //模拟眼球的位置
                Vector3 raycastStart = enemy.transform.position + Vector3.up;
                // Calculate directions for two rays with a 30° spread (15° to the left and 15° to the right)
                Vector3 leftDirection = Quaternion.Euler(0f, 30f, 0f) * enemy.transform.forward;
                Vector3 rightDirection = Quaternion.Euler(0f, -30f, 0f) * enemy.transform.forward;
                Vector3 raycastDirection = enemy.transform.forward;
                Vector3 leftSideDirection = Quaternion.Euler(0f, 60f, 0f) * enemy.transform.forward;
                Vector3 rightSideDirection = Quaternion.Euler(0f, -60f, 0f) * enemy.transform.forward;
                
                RaycastHit hit;
                if (Physics.Raycast(raycastStart, leftDirection, out hit, attackRange * 0.8f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, rightDirection, out hit, attackRange * 0.8f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, raycastDirection, out hit, attackRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, leftSideDirection, out hit, attackRange * 0.5f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, rightSideDirection, out hit, attackRange * 0.5f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    /// <summary>
    /// Scene里面画线，方便可视化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 raycastStart = transform.position + Vector3.up;
        Vector3 leftDirection = Quaternion.Euler(0f, 30f, 0f) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0f, -30f, 0f) * transform.forward;
        Vector3 raycastDirection = transform.forward;
        Vector3 leftSideDirection = Quaternion.Euler(0f, 60f, 0f) * transform.forward;
        Vector3 rightSideDirection = Quaternion.Euler(0f, -60f, 0f) * transform.forward;
        Gizmos.DrawRay(raycastStart, leftDirection * attackRange * 0.8f);
        Gizmos.DrawRay(raycastStart, rightDirection * attackRange * 0.8f);
        Gizmos.DrawRay(raycastStart, raycastDirection * attackRange);
        Gizmos.DrawRay(raycastStart, leftSideDirection * attackRange * 0.5f);
        Gizmos.DrawRay(raycastStart, rightSideDirection * attackRange * 0.5f);
    }
}

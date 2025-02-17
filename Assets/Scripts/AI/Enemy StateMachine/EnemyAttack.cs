using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;

public class EnemyAttack : NetworkBehaviour
{
    public int attackDamage = 100;
    public float attackRange = 3f;
    [HideInInspector] public float lastAttackTime = 0f;
    [HideInInspector] public float lastAttackPreDelayTime = 0f;
    public float attackPreDelay = 0.5f;
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

    public bool ShouldAttackBasedOnChasingEnemy(ChasingEnemy enemy)
    {
        // First check if target player exists and is within attack range
        if (enemy.targetPlayer != null)
        {
            float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);
            if (distanceToTarget <= attackRange)
            {
                // Check if there are obstacles between enemy and player
                //Vector3 directionToPlayer = (enemy.targetPlayer.transform.position - enemy.transform.position).normalized;
                Vector3 raycastStart = enemy.transform.position + Vector3.up;
                // Calculate directions for two rays with a 30° spread (15° to the left and 15° to the right)
                Vector3 leftDirection = Quaternion.Euler(0f, 15f, 0f) * enemy.transform.forward;
                Vector3 rightDirection = Quaternion.Euler(0f, -15f, 0f) * enemy.transform.forward;
                Vector3 raycastDirection = enemy.transform.forward;
                RaycastHit hit;
                if (Physics.Raycast(raycastStart, leftDirection, out hit, attackRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, rightDirection, out hit, attackRange))
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
            }
        }

        return false;
    }
}

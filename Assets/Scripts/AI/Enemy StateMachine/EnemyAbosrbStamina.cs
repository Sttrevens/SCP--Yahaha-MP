using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
using Unity.VisualScripting;
using LPSurvivalEngine;

public class EnemyAbsorbStamina : NetworkBehaviour
{
    public float absorbRange = 6f;

    [SerializeField] private float absorbRate = 10f;
    [HideInInspector] public Enemy enemy;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && (enemy.CurrentState is ChasingState && ShouldAbsorbPlayerStaminaBasedOnChasingEnemy(enemy.GetComponent<ChasingEnemy>())))
        {
            ReducePlayerStamina();
        }
    }

    public bool ShouldAbsorbPlayerStaminaBasedOnChasingEnemy(ChasingEnemy enemy)
    {
        // First check if target player exists and is within attack range
        if (enemy.targetPlayer != null)
        {
            float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);
            if (distanceToTarget <= absorbRange)
            {
                // Check if there are obstacles between enemy and player
                //Vector3 directionToPlayer = (enemy.targetPlayer.transform.position - enemy.transform.position).normalized;
                Vector3 raycastStart = enemy.transform.position + Vector3.up;
                // Calculate directions for two rays with a 30° spread (15° to the left and 15° to the right)
                Vector3 leftDirection = Quaternion.Euler(0f, 30f, 0f) * enemy.transform.forward;
                Vector3 rightDirection = Quaternion.Euler(0f, -30f, 0f) * enemy.transform.forward;
                Vector3 raycastDirection = enemy.transform.forward;
                Vector3 leftSideDirection = Quaternion.Euler(0f, 60f, 0f) * enemy.transform.forward;
                Vector3 rightSideDirection = Quaternion.Euler(0f, -60f, 0f) * enemy.transform.forward;
                
                RaycastHit hit;
                if (Physics.Raycast(raycastStart, leftDirection, out hit, absorbRange * 0.8f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, rightDirection, out hit, absorbRange * 0.8f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, raycastDirection, out hit, absorbRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, leftSideDirection, out hit, absorbRange * 0.5f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                if (Physics.Raycast(raycastStart, rightSideDirection, out hit, absorbRange * 0.5f))
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

    void ReducePlayerStamina()
    {
        HealthSystem playerHealth = enemy.GetComponent<ChasingEnemy>().targetPlayer.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.stamina.Subtract(absorbRate * Time.fixedDeltaTime);
        }
        else
        {
            Debug.Log("Player Health is Null,");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 raycastStart = transform.position + Vector3.up;
        Vector3 leftDirection = Quaternion.Euler(0f, 30f, 0f) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0f, -30f, 0f) * transform.forward;
        Vector3 raycastDirection = transform.forward;
        Vector3 leftSideDirection = Quaternion.Euler(0f, 60f, 0f) * transform.forward;
        Vector3 rightSideDirection = Quaternion.Euler(0f, -60f, 0f) * transform.forward;
        Gizmos.DrawRay(raycastStart, leftDirection * absorbRange * 0.8f);
        Gizmos.DrawRay(raycastStart, rightDirection * absorbRange * 0.8f);
        Gizmos.DrawRay(raycastStart, raycastDirection * absorbRange);
        Gizmos.DrawRay(raycastStart, leftSideDirection * absorbRange * 0.5f);
        Gizmos.DrawRay(raycastStart, rightSideDirection * absorbRange * 0.5f);
    }
}

using UnityEngine;
using Fusion;

public class ChasingState : EnemyBaseState
{
    public override void EnterState(EnemyAI enemy)
    {
        // Set the agent speed for chasing state
        enemy.agent.speed = enemy.chasingSpeed;
    }

    public override void UpdateState(EnemyAI enemy)
    {
        if (enemy.HasStateAuthority)
        {
            // Check if the enemy should attack the player
            if (enemy.ShouldAttack(enemy))
            {
            // Switch to AttackingState if conditions are met
            enemy.SwitchState(GetComponent<AttackingState>());
            return; // Exit early as we've already handled the state change
        }

        // Continue chasing the player if no attack is triggered
        if (enemy.targetPlayer != null && enemy.targetPlayer.tag == "Player")
        {
            enemy.agent.SetDestination(enemy.targetPlayer.transform.position);

            // If the player is within the attack range, switch to AttackingState
            if (enemy.ShouldAttack(enemy))
            {
                enemy.SwitchState(GetComponent<AttackingState>());
            }
            else if (Vector3.Distance(enemy.transform.position, enemy.targetPlayer.transform.position) > enemy.detectionRange)
            {
                // If the player is out of detection range, switch to PatrollingState
                enemy.SwitchState(GetComponent<PatrollingState>());
                enemy.targetPlayer = null;
            }
            else if (enemy.CheckForDestroyableObstacle() != null) {enemy.SwitchState(GetComponent<DestroyingObstacleState>());}
        }
        else
        {
            // If no player is detected, switch to PatrollingState
            enemy.SwitchState(GetComponent<PatrollingState>());
        }

        if (enemy._animatorManager != null)
            enemy._animatorManager.isChasing = 1;
            
        // Rotate towards the player's position (or the destination if no player)
        //enemy.RotateTowards(enemy.targetPlayer?.transform.position ?? enemy.agent.destination);
    }
    }

    public override void ExitState(EnemyAI enemy)
    {
        // No specific exit logic needed for ChasingState
    }
}
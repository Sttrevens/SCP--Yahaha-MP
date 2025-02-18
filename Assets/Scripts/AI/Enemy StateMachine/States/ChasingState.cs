using UnityEngine;
using Fusion;

public class ChasingState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);

        // Set the agent speed for chasing state
        EnemyMovement.agent.speed = ChasingEnemy.chasingSpeed;
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            // Check if the enemy should attack the player
            if (EnemyAttack.ShouldAttackBasedOnChasingEnemy(ChasingEnemy))
            {
            // Switch to AttackingState if conditions are met
            enemy.SwitchState(new AttackingState());
            return; // Exit early as we've already handled the state change
        }

        // Continue chasing the player if no attack is triggered
        if (ChasingEnemy.targetPlayer != null && ChasingEnemy.targetPlayer.tag == "Player")
        {
            EnemyMovement.agent.SetDestination(ChasingEnemy.targetPlayer.transform.position);

            if (Vector3.Distance(enemy.transform.position, ChasingEnemy.targetPlayer.transform.position) > ChasingEnemy.detectionRange)
            {
                // If the player is out of detection range, switch to PatrollingState
                enemy.SwitchState(new PatrollingState());
                ChasingEnemy.targetPlayer = null;
            }
            //else if (enemy.CheckForDestroyableObstacle() != null) {enemy.SwitchState(new DestroyingObstacleState());}
        }
        else
        {
            // If no player is detected, switch to PatrollingState
            enemy.SwitchState(new PatrollingState());
        }

        if (enemy._animatorManager != null)
            enemy._animatorManager.isChasing++; 
            
        // Smoothly rotate enemy to face the target player if one exists
        if (ChasingEnemy.targetPlayer != null)
        {
            Vector3 directionToTarget = ChasingEnemy.targetPlayer.transform.position - enemy.transform.position;
            directionToTarget.y = 0; // Keep rotation on the horizontal plane
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
    }

    public override void ExitState(Enemy enemy)
    {
        // No specific exit logic needed for ChasingState
    }
}
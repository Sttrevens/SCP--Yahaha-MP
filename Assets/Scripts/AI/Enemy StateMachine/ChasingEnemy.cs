using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChasingEnemy : EnemyMovement
{
    [HideInInspector] public GameObject targetPlayer;
    public float chasingSpeed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (PlayerInSight() && enemy.CurrentState is PatrollingState)
        {
            Debug.Log("[EnemyAI] Player detected while patrolling, switching to chase state");
            enemy.SwitchState(new ChasingState());
            agent.speed = chasingSpeed;
        }
    }
    
    public bool PlayerInSight()
    {
        if (targetPlayer != null)
        {
            if (targetPlayer.tag != "Player")
                return false;
            else
                return true;
        }

        Debug.Log("[EnemyAI] Checking for players in sight...");
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            Vector3 horizontalToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);

            float horizontalAngle = Vector3.Angle(transform.forward, horizontalToPlayer);
            float verticalAngle = Vector3.Angle(toPlayer, horizontalToPlayer);

            Debug.Log($"[EnemyAI] Checking angles - Horizontal: {horizontalAngle}, Vertical: {verticalAngle}");

            if (horizontalAngle < fieldOfViewAngleHorizontal / 2 && verticalAngle < fieldOfViewAngleVertical / 2)
            {
                Debug.Log("[EnemyAI] Player within FOV angles");
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, sensingRadius, toPlayer.normalized, out hit, detectionRange))
                {
                    if (hit.collider.gameObject == player)
                    {
                        Debug.Log("[EnemyAI] Player detected via SphereCast");
                        targetPlayer = player;
                        return true;
                    }
                }

                float distanceToPlayer = toPlayer.magnitude;
                Debug.Log($"[EnemyAI] Distance to player: {distanceToPlayer}");
                
                if (Vector3.Angle(transform.forward, horizontalToPlayer) <= fieldOfViewAngleHorizontal / 2)
                {
                    Debug.Log("[EnemyAI] Player within horizontal FOV");
                    if (distanceToPlayer <= detectionRange)
                    {
                        Debug.Log("[EnemyAI] Player within detection range");
                        if (Physics.CheckSphere(player.transform.position, sensingRadius))
                        {
                            Debug.Log("[EnemyAI] Player detected via CheckSphere");
                            targetPlayer = player;
                            return true;
                        }
                    }
                }
            }
        }
        Debug.Log("[EnemyAI] No players detected in sight");
        return false;
    }
}

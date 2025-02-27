using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChasingEnemy : EnemyMovement
{
    [HideInInspector] public GameObject targetPlayer;
    [HideInInspector] public Vector3 soundTargetPosition;
    public float chasingSpeed = 5f;
    public float hearingRadius = 10f;
    public Transform eyeTransform;

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
    
    public float stepAngle = 5f; // 多射线之间的步进角度

    [Header( "Gizmos" )]
    public bool drawGizmos = false; // Exposed variable to control drawing

    private void OnDrawGizmos()
    {
        if (!drawGizmos || eyeTransform == null) return;

        Vector3 rayStartPosition = eyeTransform.position;

        if (targetPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayStartPosition, targetPlayer.transform.position);
        }

        Gizmos.color = Color.yellow;

        // Full cone rays (模拟视野检测)
        for (float horizontal = -fieldOfViewAngleHorizontal / 2; horizontal <= fieldOfViewAngleHorizontal / 2; horizontal += stepAngle)
        {
            for (float vertical = -fieldOfViewAngleVertical / 2; vertical <= fieldOfViewAngleVertical / 2; vertical += stepAngle)
            {
                Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * eyeTransform.forward;
                Gizmos.DrawRay(rayStartPosition, rayDirection * detectionRange);
            }
        }
    }

    public bool PlayerInSight()
    {
        if (eyeTransform == null)
            return false;

        if (targetPlayer != null && targetPlayer.tag != "Player")
        {
            targetPlayer = null;
            return false;
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Vector3 toPlayer = player.transform.position - eyeTransform.position;
            float distanceToPlayer = toPlayer.magnitude;

            if (distanceToPlayer > detectionRange)
                continue;

            float horizontalAngle = Vector3.Angle(eyeTransform.forward, toPlayer);
            if (horizontalAngle > fieldOfViewAngleHorizontal / 2f)
                continue;

            float verticalAngle = Vector3.Angle(eyeTransform.forward, toPlayer);
            if (verticalAngle > fieldOfViewAngleVertical / 2f)
                continue;

            // 使用视锥判断玩家是否在视野范围内
            for (float horizontal = -fieldOfViewAngleHorizontal / 2; horizontal <= fieldOfViewAngleHorizontal / 2; horizontal += stepAngle)
            {
                for (float vertical = -fieldOfViewAngleVertical / 2; vertical <= fieldOfViewAngleVertical / 2; vertical += stepAngle)
                {
                    Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * eyeTransform.forward;

                    if (Physics.Raycast(eyeTransform.position, rayDirection, out RaycastHit hit, detectionRange))
                    {
                        if (hit.collider.gameObject == player)
                        {
                            targetPlayer = player;
                            return true;
                        }
                    }
                }
            }
        }
        
        targetPlayer = null;
        return false;
    }

    public bool PlayerHeard()
    {
        Debug.Log("[EnemyAI] Checking for players by sound...");
        Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") && collider.transform.Find("Model").GetComponent<AudioSource>().isPlaying)
            {
                Debug.Log("[EnemyAI] Player detected by sound");
                soundTargetPosition = collider.transform.position;
                return true;
            }
        }
        Debug.Log("[EnemyAI] No players detected by sound");
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
using UnityEngine.Serialization;

public class EnemyMovement : NetworkBehaviour
{
    public NavMeshAgent agent;

    [Space] [Header("AI Logic")] [Space] public float detectionRange = 10f;
    
    public float patrollingSpeed = 2f;

    public float fieldOfViewAngleHorizontal = 180f;
    public float fieldOfViewAngleVertical = 90f;
    public float sensingRadius = 3f;
    [HideInInspector] public int currentPatrolIndex = 0;

    [HideInInspector] [FormerlySerializedAs("_enemy")] public Enemy enemy;

    public enum PatrolMode
    {
        FixedPoints,
        RandomCircle,
        RandomRectangle
    }

    public PatrolMode patrolMode = PatrolMode.FixedPoints;
    public Transform[] patrolPoints;
    public float patrolRange = 10f;
    public float patrolWidth = 10f;
    public float patrolHeight = 10f;
    public float waitTimeAtPatrolPoint = 2f;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }

    public override void Spawned()
    {
        //players = GameObject.FindGameObjectsWithTag("Player");
        enemy.SwitchState(new PatrollingState());
    }

    public override void FixedUpdateNetwork()
    {
        RotateTowardsMovementDirection();
    }

    public void RotateTowardsMovementDirection()
    {
        // 若当前有移动速度
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            // 只在水平方向旋转
            Vector3 moveDirection = agent.velocity;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            // 计算应该朝向的目标旋转
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // 通过球面插值让敌人平滑地转向
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
        
        Debug.Log("Velocity: " + agent.velocity);
    }
    
    public IEnumerator Patrol()
    {
        if (patrolMode == PatrolMode.FixedPoints)
        {
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < agent.stoppingDistance + 1f)
            {
                Debug.Log($"[EnemyAI] Reached patrol point {currentPatrolIndex}, waiting {waitTimeAtPatrolPoint} seconds");
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                Debug.Log($"[EnemyAI] Moving to next patrol point {currentPatrolIndex}");
            }

            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else if (patrolMode == PatrolMode.RandomCircle || patrolMode == PatrolMode.RandomRectangle)
        {
            if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance + 1f)
            {
                Debug.Log("[EnemyAI] Reached random patrol point, waiting before next point");
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                Vector3 randomPatrolPoint = GetRandomPatrolPoint();
                agent.SetDestination(randomPatrolPoint);
                Debug.Log($"[EnemyAI] Moving to new random patrol point at {randomPatrolPoint}");
            }
        }
        yield return null;
    }
    
    private Vector3 GetRandomPatrolPoint()
    {
        if (patrolMode == PatrolMode.RandomCircle)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomRadius = Random.Range(0f, patrolRange);
            float x = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
            float z = transform.position.z + randomRadius * Mathf.Sin(randomAngle);
            return new Vector3(x, transform.position.y, z);
        }
        else if (patrolMode == PatrolMode.RandomRectangle)
        {
            float randomX = Random.Range(-patrolWidth / 2f, patrolWidth / 2f);
            float randomZ = Random.Range(-patrolHeight / 2f, patrolHeight / 2f);
            return new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        int numSegments = 20;
        float angleStep = 2 * Mathf.PI / numSegments;
        for (int i = 0; i < numSegments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            Vector3 point1 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward *
                             detectionRange;
            Vector3 point2 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward *
                             detectionRange;

            Vector3 point3 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward *
                             detectionRange;
            Vector3 point4 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward *
                             detectionRange;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + point1);
            Gizmos.DrawLine(transform.position, transform.position + point2);
            Gizmos.DrawLine(transform.position, transform.position + point3);
            Gizmos.DrawLine(transform.position, transform.position + point4);

            Gizmos.DrawLine(transform.position + point1, transform.position + point3);
            Gizmos.DrawLine(transform.position + point2, transform.position + point4);
        }

        if (patrolMode == PatrolMode.RandomCircle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, patrolRange);
        }
        else if (patrolMode == PatrolMode.RandomRectangle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(patrolWidth, 0, patrolHeight));
        }
    }
}

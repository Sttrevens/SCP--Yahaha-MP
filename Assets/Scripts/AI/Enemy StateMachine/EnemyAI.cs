using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private GameObject[] players;
    public Animator animator;
    [HideInInspector]
    public int currentPatrolIndex = 0;

    [Space]
    [Header("AI Logic")]
    [Space]

    public float detectionRange = 10f;
    public float attackPreDelay = 0.5f;
    public float attackInterval = 3f;
    public float chasingSpeed = 3.5f;
    public float patrollingSpeed = 2f;

    public float fieldOfViewAngleHorizontal = 90f;
    public float fieldOfViewAngleVertical = 60f;
    public float sensingRadius = 0.1f;

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

    [Space]
    [Header("Enemy")]
    [Space]

    public int attackDamage = 20;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float attackRange = 2f;
    [HideInInspector] public float lastAttackTime = 0f;
    [HideInInspector] public float lastAttackPreDelayTime = 0f;
    [HideInInspector] public GameObject targetPlayer;

    public IEnemyState currentState;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
       currentHealth = maxHealth;
        SwitchState(new PatrollingState());
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void SwitchState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        currentState.EnterState(this);
    }

    private bool PathBlocked()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, agent.destination - transform.position, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Passable"))
            {
                return true;
            }
        }
        return false;
    }

    public GameObject CheckForDestroyableObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, agent.destination - transform.position, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Passable"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
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

    public bool PlayerInSight()
    {
        foreach (GameObject player in players)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            Vector3 horizontalToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);

            float horizontalAngle = Vector3.Angle(transform.forward, horizontalToPlayer);
            float verticalAngle = Vector3.Angle(toPlayer, horizontalToPlayer);

            if (horizontalAngle < fieldOfViewAngleHorizontal / 2 && verticalAngle < fieldOfViewAngleVertical / 2)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, sensingRadius, toPlayer.normalized, out hit, detectionRange))
                {
                    if (hit.collider.gameObject == player)
                    {
                        targetPlayer = player;
                        return true;
                    }
                }

                float distanceToPlayer = toPlayer.magnitude;
                if (Vector3.Angle(transform.forward, horizontalToPlayer) <= fieldOfViewAngleHorizontal / 2)
                {
                    if (distanceToPlayer <= detectionRange)
                    {
                        if (Physics.CheckSphere(player.transform.position, sensingRadius))
                        {
                            targetPlayer = player;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        if (!(currentState is DeadState))
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                SwitchState(new DeadState());
            }
            else if (damage >= maxHealth / 6)
            {
                SwitchState(new BeingAttackedState());
            }
        }
    }

    private void OnEnable()
    {
        FallingLogic.OnFallingAnimationStarted += OnFallingAnimationStartedHandler;
        OnGround.OnGroundAnimationStarted += OnGroundAnimationStartedHandler;
    }

    private void OnDisable()
    {
        FallingLogic.OnFallingAnimationStarted -= OnFallingAnimationStartedHandler;
        OnGround.OnGroundAnimationStarted -= OnGroundAnimationStartedHandler;
    }

    private void OnFallingAnimationStartedHandler(GameObject triggeredObject)
    {
        if (triggeredObject == animator.gameObject)
        {
            HandleDeath();
        }
    }

    private void OnGroundAnimationStartedHandler(GameObject triggeredObject)
    {
        if (triggeredObject == animator.gameObject)
        {
            EnableChopped();
        }
    }

    private void HandleDeath()
    {
        if (agent != null)
        {
            Destroy(agent);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void EnableChopped()
    {

                Destructible destructible = gameObject.AddComponent<Destructible>();
                gameObject.AddComponent<DropItem>();

                destructible.TotalHitPoints = 50f;
                destructible.CurrentHitPoints = destructible.TotalHitPoints;

        if (GetComponent<ChoppedItems>() != null && GetComponent<ChoppedItems>().destroyParticles != null)
        {
            destructible.destroyedPrefab = GetComponent<ChoppedItems>().destroyParticles;
        }
            
    }

    public void Idle()
    {
        PlayAnimation("IsIdle", true);
    }

    public IEnumerator Patrol()
    {
        if (patrolMode == PatrolMode.FixedPoints)
        {
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
            {
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else if (patrolMode == PatrolMode.RandomCircle || patrolMode == PatrolMode.RandomRectangle)
        {
            if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
            {
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                Vector3 randomPatrolPoint = GetRandomPatrolPoint();
                agent.SetDestination(randomPatrolPoint);
            }
        }

        if (PlayerInSight())
        {
            SwitchState(new ChasingState());
            agent.speed = chasingSpeed;
            yield break;
        }

        PlayAnimation("IsPatrolling", true);
        RotateTowards(agent.destination);
        yield return null;
    }

    public void DestroyObstacle()
    {
        GameObject obstacle = CheckForDestroyableObstacle();
        if (obstacle != null)
        {
            agent.isStopped = true;
            StartCoroutine(DestroyObstacleRoutine(obstacle));
        }
    }

    private IEnumerator DestroyObstacleRoutine(GameObject obstacle)
    {
        while (obstacle != null)
        {
            Destructible destructible = obstacle.GetComponent<Destructible>();
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(1f); 
            destructible.ApplyDamage(attackDamage); 
            yield return new WaitForSeconds(attackInterval); 

            if (obstacle == null) break; 
        }

        // »Ö¸´×·Öð×´Ì¬
        agent.isStopped = false;
        SwitchState(new ChasingState());
    }

    public bool ShouldAttack(EnemyAI enemy)
    {
        RaycastHit hit;
        float raycastDistance = enemy.attackRange;
        Vector3 raycastDirection = enemy.transform.forward;

        // Adjust the starting position of the raycast (move it 1 unit higher in the y-axis)
        Vector3 raycastStart = enemy.transform.position + Vector3.up; // Move 1 unit up

        // Raycast to check if a player is within detection range
        if (Physics.Raycast(raycastStart, raycastDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                enemy.targetPlayer = hit.collider.gameObject;
                return true; // Player detected, ready to attack
            }
        }

        // Player is out of detection range, no attack
        return false;
    }

    public void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void PlayAnimation(string animationName, bool isLooping)
    {
        if (animator != null)
        {
            animator.SetBool("IsPatrolling", isLooping && animationName == "FlyStationary");
            animator.SetBool("IsChasing", isLooping && animationName == "FlyForwardSlow");
            animator.SetBool("IsIdle", isLooping && animationName == "FlyStationary");
            animator.SetBool("IsDestroying", !isLooping && animationName == "BiteAttack1");
            animator.SetBool("IsDead", !isLooping && animationName == "DeathToFalling");
        }
    }

    private void OnDrawGizmos()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 raycastStart = transform.position + Vector3.up;
        Vector3 raycastDirection = transform.forward;
        Gizmos.DrawRay(raycastStart, raycastDirection * attackRange);


        int numSegments = 20;
        float angleStep = 2 * Mathf.PI / numSegments;
        for (int i = 0; i < numSegments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            Vector3 point1 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
            Vector3 point2 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

            Vector3 point3 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
            Vector3 point4 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

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

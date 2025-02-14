using DestroyIt;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : NetworkBehaviour
{
    public NavMeshAgent agent;
    [SerializeField]
    //private GameObject[] players;
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
    [Networked] public float currentHealth { get; set; } = 100f;
    public float attackRange = 2f;
    [HideInInspector] public float lastAttackTime = 0f;
    [HideInInspector] public float lastAttackPreDelayTime = 0f;
    [HideInInspector] public GameObject targetPlayer;

    [SerializeField] private bool isFlyingEnemy = false;

    public IEnemyState currentState;

    public EnemyAnimatorManager _animatorManager;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        // players = GameObject.FindGameObjectsWithTag("Player");
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public override void Spawned()
    {
        //players = GameObject.FindGameObjectsWithTag("Player");
        currentHealth = maxHealth;
        SwitchState(new PatrollingState());
    }

    private void Start()
    {
       currentHealth = maxHealth;
        SwitchState(new PatrollingState());
    }

    public override void FixedUpdateNetwork()
    {
        if (currentState != null)
        {
             currentState.UpdateState(this);

           //players = GameObject.FindGameObjectsWithTag("Player");
        }

        RotateTowardsMovementDirection();
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

    public void TakeDamage(float damage)
    {
        if (!(currentState is DeadState))
        {
            // TODO:����������Ѫ��ͬ��
            // currentHealth -= damage;
            DealDamageRpc(damage);
            if (currentHealth <= 0)
            {
                if (!isFlyingEnemy)
                {
                    HandleDeath();
                }
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
            //EnableChopped();
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
                Debug.Log($"[EnemyAI] Reached patrol point {currentPatrolIndex}, waiting {waitTimeAtPatrolPoint} seconds");
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                Debug.Log($"[EnemyAI] Moving to next patrol point {currentPatrolIndex}");
            }
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else if (patrolMode == PatrolMode.RandomCircle || patrolMode == PatrolMode.RandomRectangle)
        {
            if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
            {
                Debug.Log("[EnemyAI] Reached random patrol point, waiting before next point");
                yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                Vector3 randomPatrolPoint = GetRandomPatrolPoint();
                agent.SetDestination(randomPatrolPoint);
                Debug.Log($"[EnemyAI] Moving to new random patrol point at {randomPatrolPoint}");
            }
        }

        if (PlayerInSight())
        {
            Debug.Log("[EnemyAI] Player detected while patrolling, switching to chase state");
            SwitchState(new ChasingState());
            agent.speed = chasingSpeed;
            yield break;
        }

        PlayAnimation("IsPatrolling", true);
        //RotateTowards(agent.destination);
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
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            yield return new WaitForSeconds(1f); 
            destructible.ApplyDamage(attackDamage); 
            yield return new WaitForSeconds(attackInterval); 

            if (obstacle == null) break; 
        }

        // �ָ�׷��״̬
        agent.isStopped = false;
        SwitchState(new ChasingState());
    }

        public bool ShouldAttack(EnemyAI enemy)
    {
        // First check if target player exists and is within attack range
        if (enemy.targetPlayer != null)
        {
            float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);
            if (distanceToTarget <= enemy.attackRange)
            {
                // Check if there are obstacles between enemy and player
                Vector3 directionToPlayer = (enemy.targetPlayer.transform.position - enemy.transform.position).normalized;
                RaycastHit hit;
                Vector3 raycastStart = enemy.transform.position + Vector3.up;

                if (Physics.Raycast(raycastStart, directionToPlayer, out hit, enemy.attackRange))
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
/// 根据 NavMeshAgent 当前的移动方向来旋转敌人
/// </summary>
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void PlayAnimation(string animationName, bool isLooping)
    {
        //if (animator != null)
        //{
        //    animator.SetBool("IsPatrolling", isLooping && animationName == "FlyStationary");
        //    animator.SetBool("IsChasing", isLooping && animationName == "FlyForwardSlow");
        //    animator.SetBool("IsIdle", isLooping && animationName == "FlyStationary");
        //    animator.SetBool("IsDestroying", !isLooping && animationName == "BiteAttack1");
        //    animator.SetBool("IsDead", !isLooping && animationName == "DeathToFalling");
        //}
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        currentHealth -= damage;
    }
}

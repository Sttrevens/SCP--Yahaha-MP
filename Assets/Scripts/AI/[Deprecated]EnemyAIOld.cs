using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

    public class EnemyAIOld : MonoBehaviour
    {
        private NavMeshAgent agent;
        private GameObject[] players;
        public Animator animator;
        private int currentPatrolIndex = 0;

        [Space]
        [Header("AI Logic")]
        [Space]

        public float detectionRange = 10f; // ��Ұ��Χ
        public float attackInterval = 3f; // �ݻ��ϰ�������ʱ��
        public float chasingSpeed = 3.5f; // ׷���ٶ�
        public float patrollingSpeed = 2f; // Ѳ���ٶ�

        private bool isDestroying = false;

        public float fieldOfViewAngleHorizontal = 90f;  // ˮƽ��Ұ�Ƕ�
        public float fieldOfViewAngleVertical = 60f;  // ��ֱ��Ұ�Ƕ�
        public float sensingRadius = 0.1f;

        public enum PatrolMode
        {
            FixedPoints,  // �̶�Ѳ�ߵ�
            RandomCircle, // Բ�η�ΧѲ��
            RandomRectangle // ���η�ΧѲ��
        }

        public PatrolMode patrolMode = PatrolMode.FixedPoints; // ��ǰѲ��ģʽ
        public Transform[] patrolPoints; // Ѳ�ߵ�
        public float patrolRange = 10f; // Բ��Ѳ��ģʽ��Ѳ�߰뾶
        public float patrolWidth = 10f; // ���η�ΧѲ�ߵĿ���
        public float patrolHeight = 10f; // ���η�ΧѲ�ߵĸ߶�
        public float waitTimeAtPatrolPoint = 2f; // ÿ��Ѳ�ߵ�ȴ���ʱ��

        [Space]
        [Header("Enemy")]
        [Space]

        public float attackDamage = 20f;
        public float health = 100f; // 敌人生命值
public float attackRange = 2f; // 攻击玩家的范围
private float lastAttackTime = 0f; // 上次攻击时间
private GameObject targetPlayer; // 当前追逐的玩家


        private enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking, // 攻击状态
    BeingAttacked, // 被攻击状态
    Dead, // 死亡状态
    Idle
}

        private EnemyState currentState;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            players = GameObject.FindGameObjectsWithTag("Player");
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            currentState = EnemyState.Patrolling;
            agent.speed = patrollingSpeed;
            StartCoroutine(StateMachine());
        }

        private IEnumerator StateMachine()
{
    while (true)
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                yield return StartCoroutine(Patrol());
                break;

            case EnemyState.Chasing:
                ChasePlayer();
                break;

            case EnemyState.Attacking:
                AttackPlayer();
                break;

            case EnemyState.BeingAttacked:
                yield return new WaitForSeconds(1f); 
                currentState = EnemyState.Chasing; 
                break;

            case EnemyState.Dead:
                Dead();
                    yield break;

            case EnemyState.Idle:
                Idle();
                break;
        }

        yield return null;
    }
}

        private IEnumerator Patrol()
        {
            if (patrolMode == PatrolMode.FixedPoints)
            {
                // �̶�Ѳ�ߵ�ģʽ
                if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
                {
                    // ����Ѳ�ߵ��ȴ�һ��ʱ��
                    yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }

                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            else if (patrolMode == PatrolMode.RandomCircle || patrolMode == PatrolMode.RandomRectangle)
            {
                // ��Χ�����Ѳ��ģʽ
                if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
                {
                    // ����Ŀ����ȴ�һ��ʱ��
                    yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                    Vector3 randomPatrolPoint = GetRandomPatrolPoint();
                    agent.SetDestination(randomPatrolPoint);
                }
            }

            if (PlayerInSight())
            {
                currentState = EnemyState.Chasing;
                agent.speed = chasingSpeed; // �л�Ϊ׷���ٶ�
            }

            // ��Ѳ���м���Ƿ����ϰ����赲
            // if (PathBlocked())
            // {
            //     GameObject obstacle = CheckForDestroyableObstacle();
            //     if (obstacle != null)
            //     {
            //         currentState = EnemyState.Destroying;
            //     }
            // }

            // ����Ѳ�߶���
            PlayAnimation("IsPatrolling", true);
            // ���Ƴ���Ŀ�귽��
            RotateTowards(agent.destination);
        }

        private void ChasePlayer()
{
    if (targetPlayer != null)
    {
        agent.SetDestination(targetPlayer.transform.position);

        // 如果玩家距离敌人小于攻击范围，进入攻击状态
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackRange)
        {
            currentState = EnemyState.Attacking; // 切换到攻击状态
        }
        else if (Vector3.Distance(transform.position, targetPlayer.transform.position) > detectionRange)
        {
            currentState = EnemyState.Patrolling;
            agent.speed = patrollingSpeed;
            targetPlayer = null;
        }

        // 播放追击动画
        PlayAnimation("IsChasing", true);
        RotateTowards(targetPlayer.transform.position);
    }
    else
    {
        currentState = EnemyState.Patrolling;
        agent.speed = patrollingSpeed;
    }
}

private void AttackPlayer()
{
    if (targetPlayer != null && Time.time - lastAttackTime >= attackInterval)
    {
        lastAttackTime = Time.time; // 更新上次攻击时间

        // 这里调用玩家的受伤方法 (你可以自己实现)
        // targetPlayer.GetComponent<PlayerHealth>().TakeDamage(attackDamage); 

        animator.SetTrigger("Attack");

        // 攻击后返回追逐状态
        currentState = EnemyState.Chasing;
    }
}


        private void DestroyObstacle()
        {
            // �����ϰ�����һ�����Դݻٵ�����
            GameObject obstacle = CheckForDestroyableObstacle();
            if (obstacle != null)
            {
                // �ڴݻ��ϰ���ǰͣ����
                agent.isStopped = true;
                StartCoroutine(DestroyObstacleRoutine(obstacle));
            }

            // ���Ŵݻ��ϰ��ﶯ��
            PlayAnimation("IsDestroying", true); // ����ֻ����һ��
        }

        private IEnumerator DestroyObstacleRoutine(GameObject obstacle)
{
    while (obstacle != null) // 如果障碍物仍然存在
    {
        Destructible destructible = obstacle.GetComponent<Destructible>();

        // 播放攻击动画
        animator.SetTrigger("Attack");  // 使用Trigger触发攻击动画
       
        yield return new WaitForSeconds(1f);
        
        destructible.ApplyDamage(attackDamage);
        
        yield return new WaitForSeconds(attackInterval);

        // 检查物体是否仍然在场景中
        if (obstacle == null)
        {
            // 如果被销毁，跳出循环
            break;
        }
    }

    // 如果障碍物被摧毁，停止追逐并切换状态
    agent.isStopped = false;
    currentState = EnemyState.Chasing; // 切换到追逐状态
}


        private void Idle()
        {
            PlayAnimation("IsIdle", true);
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

        private GameObject CheckForDestroyableObstacle()
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
                // ѡ��ΧѲ��ģʽ�µ�����㣨Բ�Σ�
                float randomAngle = Random.Range(0f, 360f);
                float randomRadius = Random.Range(0f, patrolRange);

                float x = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
                float z = transform.position.z + randomRadius * Mathf.Sin(randomAngle);

                return new Vector3(x, transform.position.y, z);
            }
            else if (patrolMode == PatrolMode.RandomRectangle)
            {
                // ���η�ΧѲ�ߵ������
                float randomX = Random.Range(-patrolWidth / 2f, patrolWidth / 2f);
                float randomZ = Random.Range(-patrolHeight / 2f, patrolHeight / 2f);

                return new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            }

            return transform.position; // Ĭ�Ϸ��ص�ǰλ��
        }

        private bool PlayerInSight()
{
    foreach (GameObject player in players)
    {
        Vector3 toPlayer = player.transform.position - transform.position;

        // Get the horizontal direction vector (ignore the vertical component)
        Vector3 horizontalToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);
        
        // Calculate the horizontal and vertical angles between the enemy and the player
        float horizontalAngle = Vector3.Angle(transform.forward, horizontalToPlayer);
        float verticalAngle = Vector3.Angle(toPlayer, horizontalToPlayer);

        // Check if the player is within the field of view angle (both horizontal and vertical)
        if (horizontalAngle < fieldOfViewAngleHorizontal / 2 && verticalAngle < fieldOfViewAngleVertical / 2)
        {
            // Perform a SphereCast for ball-shaped detection range
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, sensingRadius, toPlayer.normalized, out hit, detectionRange))
            {
                // Check if the SphereCast hit the player
                if (hit.collider.gameObject == player)
                {
                    targetPlayer = player; // Set the target player
                    return true; // Player is detected
                }
            }

            // Additionally, perform a cone-shaped detection (like the camera's field of view)
            float coneAngle = Mathf.Deg2Rad * fieldOfViewAngleHorizontal / 2;
            float distanceToPlayer = toPlayer.magnitude;

            // Check if the player is within the detection cone using the dot product
            if (Vector3.Angle(transform.forward, horizontalToPlayer) <= fieldOfViewAngleHorizontal / 2)
            {
                // Check if the player is within a spherical detection range
                if (distanceToPlayer <= detectionRange)
                {
                    // Use a trigger zone (Sphere) detection if player enters range
                    if (Physics.CheckSphere(player.transform.position, sensingRadius))
                    {
                        targetPlayer = player; // Set the target player
                        return true; // Player detected within cone and sphere range
                    }
                }
            }
        }
    }

    return false; // No player detected
}

public void TakeDamage(float damage)
{
    if (currentState != EnemyState.Dead)
    {
            StopAllCoroutines();
            Debug.Log("Hit!");
        health -= damage; // 减少生命值
            animator.SetTrigger("Hit");
            StartCoroutine(StateMachine());
            currentState = EnemyState.BeingAttacked; 

        if (health <= 0)
        {
            currentState = EnemyState.Dead;
        }
    }
}

    private void Dead()
    {
        animator.SetBool("IsDead", true);
    }

    private void OnEnable()
    {
        FallingLogic.OnFallingAnimationStarted += OnFallingAnimationStartedHandler;
    }

    private void OnDisable()
    {
        FallingLogic.OnFallingAnimationStarted -= OnFallingAnimationStartedHandler;
    }

    private void OnFallingAnimationStartedHandler(GameObject triggeredObject)
    {
        if (triggeredObject == animator.gameObject)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Fuck");

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


    private void RotateTowards(Vector3 target)
        {
            // ����Ŀ�귽���õ��˳���Ŀ��
            Vector3 direction = (target - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private void PlayAnimation(string animationName, bool isLooping)
        {
            // ����״̬������Ӧ�Ķ���
            if (animator != null)
            {
                // ���ò�������������ѭ������
                animator.SetBool("IsPatrolling", isLooping && animationName == "FlyStationary");
                animator.SetBool("IsChasing", isLooping && animationName == "FlyForwardSlow");
                animator.SetBool("IsIdle", isLooping && animationName == "FlyStationary");
                animator.SetBool("IsDestroying", !isLooping && animationName == "BiteAttack1");
                animator.SetBool("IsDead", !isLooping && animationName == "DeathToFalling");
            }
        }

        private void OnDrawGizmos()
        {
            // ���Ƶ�����Ұ��Χ
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // ����3D��Ұ�Ƕȷ�Χ����ʾ��Բ׶����״��
            int numSegments = 20; // ���ڿ��ƻ���Բ׶��Բ�ܵķֶ��������ɵ���ʹ��ʾ��ƽ��
            float angleStep = 2 * Mathf.PI / numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = (i + 1) * angleStep;

                // ����Բ׶������ϵ����������꣨����ˮƽ�ʹ�ֱ��Ұ�Ƕȣ�
                Vector3 point1 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
                Vector3 point2 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

                Vector3 point3 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
                Vector3 point4 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

                // ����Բ׶����������
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + point1);
                Gizmos.DrawLine(transform.position, transform.position + point2);
                Gizmos.DrawLine(transform.position, transform.position + point3);
                Gizmos.DrawLine(transform.position, transform.position + point4);

                Gizmos.DrawLine(transform.position + point1, transform.position + point3);
                Gizmos.DrawLine(transform.position + point2, transform.position + point4);
            }

            // ����Ѳ�߷�Χ
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
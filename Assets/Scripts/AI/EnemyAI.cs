using DestroyIt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    public class EnemyAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        private GameObject[] players;
        public Animator animator;
        private int currentPatrolIndex = 0;

        [Space]
        [Header("AI Logic")]
        [Space]

        public float detectionRange = 10f; // 视野范围
        public float destroyRange = 1f; // 摧毁障碍物的距离
        public float destroyTime = 3f; // 摧毁障碍物所需时间
        public float chasingSpeed = 3.5f; // 追逐速度
        public float patrollingSpeed = 2f; // 巡逻速度

        private bool isDestroying = false;

        public float fieldOfViewAngleHorizontal = 90f;  // 水平视野角度
        public float fieldOfViewAngleVertical = 60f;  // 垂直视野角度
        public float sphereRadius = 0.1f;

        public enum PatrolMode
        {
            FixedPoints,  // 固定巡逻点
            RandomCircle, // 圆形范围巡逻
            RandomRectangle // 矩形范围巡逻
        }

        public PatrolMode patrolMode = PatrolMode.FixedPoints; // 当前巡逻模式
        public Transform[] patrolPoints; // 巡逻点
        public float patrolRange = 10f; // 圆形巡逻模式的巡逻半径
        public float patrolWidth = 10f; // 矩形范围巡逻的宽度
        public float patrolHeight = 10f; // 矩形范围巡逻的高度
        public float waitTimeAtPatrolPoint = 2f; // 每个巡逻点等待的时间

        [Space]
        [Header("Enemy")]
        [Space]

        public float attackDamage = 20f;

        private enum EnemyState
        {
            Patrolling,
            Chasing,
            Destroying,
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

                    case EnemyState.Destroying:
                        DestroyObstacle();
                        break;

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
                // 固定巡逻点模式
                if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
                {
                    // 到达巡逻点后等待一段时间
                    yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }

                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            else if (patrolMode == PatrolMode.RandomCircle || patrolMode == PatrolMode.RandomRectangle)
            {
                // 范围内随机巡逻模式
                if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
                {
                    // 到达目标点后等待一段时间
                    yield return new WaitForSeconds(waitTimeAtPatrolPoint);
                    Vector3 randomPatrolPoint = GetRandomPatrolPoint();
                    agent.SetDestination(randomPatrolPoint);
                }
            }

            if (PlayerInSight())
            {
                currentState = EnemyState.Chasing;
                agent.speed = chasingSpeed; // 切换为追逐速度
            }

            // 在巡逻中检查是否有障碍物阻挡
            if (PathBlocked())
            {
                GameObject obstacle = CheckForDestroyableObstacle();
                if (obstacle != null)
                {
                    currentState = EnemyState.Destroying;
                }
            }

            // 播放巡逻动画
            PlayAnimation("IsPatrolling", true);
            // 控制朝向目标方向
            RotateTowards(agent.destination);
        }

        private GameObject targetPlayer;

        private void ChasePlayer()
        {
            if (targetPlayer != null)
            {
                agent.SetDestination(targetPlayer.transform.position);

                // 追逐过程中检查是否发现目标玩家离开了视野
                if (Vector3.Distance(transform.position, targetPlayer.transform.position) > detectionRange)
                {
                    currentState = EnemyState.Patrolling;
                    agent.speed = patrollingSpeed;
                    targetPlayer = null; // 丢失目标后，重置目标玩家对象为null
                }

                // 追逐中遇到障碍物时检查是否可以摧毁
                if (PathBlocked())
                {
                    GameObject obstacle = CheckForDestroyableObstacle();
                    if (obstacle != null)
                    {
                        currentState = EnemyState.Destroying;
                    }
                }

                // 播放追逐动画
                PlayAnimation("IsChasing", true);
                // 控制朝向目标方向
                RotateTowards(targetPlayer.transform.position);
            }
            else
            {
                // 如果没有目标玩家（可能之前追逐的玩家丢失了且没发现新目标），可以切换回巡逻状态等逻辑处理
                currentState = EnemyState.Patrolling;
                agent.speed = patrollingSpeed;
            }
        }

        private void DestroyObstacle()
        {
            // 假设障碍物是一个可以摧毁的物体
            GameObject obstacle = CheckForDestroyableObstacle();
            if (obstacle != null)
            {
                // 在摧毁障碍物前停下来
                agent.isStopped = true;
                StartCoroutine(DestroyObstacleRoutine(obstacle));
            }

            // 播放摧毁障碍物动画
            PlayAnimation("IsDestroying", false); // 动画只播放一次
        }

        private IEnumerator DestroyObstacleRoutine(GameObject obstacle)
        {
            // 模拟摧毁障碍物的过程
            yield return new WaitForSeconds(destroyTime);
            obstacle.GetComponent<Destructible>().ApplyDamage(attackDamage);
            agent.isStopped = false;
            currentState = EnemyState.Chasing; // 摧毁完后继续追逐
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
                if (hit.collider.CompareTag("Obstacle"))
                {
                    return true;
                }
            }
            return false;
        }

        private GameObject CheckForDestroyableObstacle()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, agent.destination - transform.position, out hit, destroyRange))
            {
                if (hit.collider.CompareTag("DestroyableObstacle"))
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
                // 选择范围巡逻模式下的随机点（圆形）
                float randomAngle = Random.Range(0f, 360f);
                float randomRadius = Random.Range(0f, patrolRange);

                float x = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
                float z = transform.position.z + randomRadius * Mathf.Sin(randomAngle);

                return new Vector3(x, transform.position.y, z);
            }
            else if (patrolMode == PatrolMode.RandomRectangle)
            {
                // 矩形范围巡逻的随机点
                float randomX = Random.Range(-patrolWidth / 2f, patrolWidth / 2f);
                float randomZ = Random.Range(-patrolHeight / 2f, patrolHeight / 2f);

                return new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            }

            return transform.position; // 默认返回当前位置
        }

        private bool PlayerInSight()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                Vector3 toPlayer = player.transform.position - transform.position;

                // 获取目标方向向量在水平面上的投影向量
                Vector3 horizontalToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);
                // 计算水平面上的夹角
                float horizontalAngle = Vector3.Angle(transform.forward, horizontalToPlayer);
                // 计算俯仰角度
                float verticalAngle = Vector3.Angle(toPlayer, horizontalToPlayer);

                // 判断是否在水平和垂直视野角度范围内
                if (horizontalAngle < fieldOfViewAngleHorizontal / 2 && verticalAngle < fieldOfViewAngleVertical / 2)
                {
                    RaycastHit hit;
                    // 使用SphereCast进行3D视野范围的射线检测
                    if (Physics.SphereCast(transform.position, sphereRadius, toPlayer.normalized, out hit, detectionRange))
                    {
                        if (hit.collider.gameObject == player)
                        {
                            targetPlayer = player; // 将发现的玩家赋值给目标玩家变量
                            return true; // 如果有任意一个玩家在视野内就返回true
                        }
                    }
                }
            }
            return false;
        }


        private void RotateTowards(Vector3 target)
        {
            // 计算目标方向并让敌人朝向目标
            Vector3 direction = (target - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private void PlayAnimation(string animationName, bool isLooping)
        {
            // 根据状态播放相应的动画
            if (animator != null)
            {
                // 设置布尔参数来控制循环动画
                animator.SetBool("IsPatrolling", isLooping && animationName == "FlyStationary");
                animator.SetBool("IsChasing", isLooping && animationName == "FlyForwardSlow");
                animator.SetBool("IsIdle", isLooping && animationName == "FlyStationary");
                animator.SetBool("IsDestroying", !isLooping && animationName == "BiteAttack1");
            }
        }

        private void OnDrawGizmos()
        {
            // 绘制敌人视野范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // 绘制3D视野角度范围（简单示意圆锥体形状）
            int numSegments = 20; // 用于控制绘制圆锥体圆周的分段数量，可调整使显示更平滑
            float angleStep = 2 * Mathf.PI / numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = (i + 1) * angleStep;

                // 计算圆锥体表面上的两个点坐标（基于水平和垂直视野角度）
                Vector3 point1 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
                Vector3 point2 = Quaternion.Euler(-fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

                Vector3 point3 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle1, 0) * transform.forward * detectionRange;
                Vector3 point4 = Quaternion.Euler(fieldOfViewAngleVertical / 2, angle2, 0) * transform.forward * detectionRange;

                // 绘制圆锥体表面的线条
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + point1);
                Gizmos.DrawLine(transform.position, transform.position + point2);
                Gizmos.DrawLine(transform.position, transform.position + point3);
                Gizmos.DrawLine(transform.position, transform.position + point4);

                Gizmos.DrawLine(transform.position + point1, transform.position + point3);
                Gizmos.DrawLine(transform.position + point2, transform.position + point4);
            }

            // 绘制巡逻范围
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
}

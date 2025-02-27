using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Root : NetworkBehaviour
{
    public float detectionRange = 10f;
    private EnemyBaseState currentStateBehavior;
    private Enemy enemy;

    public Transform eyeTransform; // New Transform reference for the raycast starting point and direction

    public float fieldOfViewAngleHorizontal = 120f;
    public float fieldOfViewAngleVertical = 90f;

    [HideInInspector] public GameObject targetPlayer;
    public float stepAngle = 5f; // 多射线之间的步进角度

    public float pullRadius = 5f;       // 牵制范围：腐根为中心半径
    public float breakFreeThreshold = 2f; // 挣脱角速度的累计阈值
    public float escapeCooldown = 5f;  // 逃脱后的冷却时间
    private bool isRestraining = false; // 是否正在牵制
    private bool isCooldown = false;   // 是否处于冷却中

    private Coroutine restrainCoroutine;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

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

    public override void FixedUpdateNetwork()
    {
        if (isCooldown || isRestraining)
        {
            // 冷却或正在牵制状态下，无其他行为
            return;
        }

        if (PlayerInSight())
        {
            // 检测到玩家，开始牵制
            if (restrainCoroutine == null)
            {
                restrainCoroutine = StartCoroutine(RestrainPlayer());
            }
        }
        else
        {
            // 状态重置
            enemy._animatorManager.isChasing = false;
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

    private IEnumerator RestrainPlayer()
    {
        if (targetPlayer == null)
            yield break;

        isRestraining = true;
        enemy._animatorManager.isChasing = true;

        Debug.Log("开始牵制玩家：" + targetPlayer.name);

        float accumulatedAngle = 0f;
        Vector3 centerPosition = transform.position;

        while (isRestraining && targetPlayer != null)
        {
            // 计算玩家距离和角速度
            Vector3 toPlayer = targetPlayer.transform.position - centerPosition;
            float distanceToPlayer = toPlayer.magnitude;

            // 限制玩家移动范围
            if (distanceToPlayer > pullRadius)
            {
                Debug.Log("玩家试图离开牵制范围，强制推回！");
                Vector3 clampedPosition = centerPosition + toPlayer.normalized * pullRadius;
                targetPlayer.transform.position = clampedPosition; // 强制限制玩家位置
            }

            // 玩家挣脱逻辑
            float angleDelta = Vector3.SignedAngle(toPlayer.normalized, -transform.forward.normalized, Vector3.up);
            accumulatedAngle += Mathf.Abs(angleDelta * Time.deltaTime);

            if (accumulatedAngle >= breakFreeThreshold)
            {
                Debug.Log("玩家挣脱成功！");
                break; // 玩家挣脱
            }

            // 持续牵制效果（玩家掉san等逻辑留空）
            // TODO: 玩家持续掉SAN逻辑由其他部分提供
            yield return null;
        }

        Debug.Log("牵制结束，进入冷却状态。");
        StartCoroutine(EnterCooldown());
    }

    private IEnumerator EnterCooldown()
    {
        isRestraining = false;

        if (targetPlayer != null)
        {
            targetPlayer = null; // 清除当前目标
        }

        Debug.Log("进入冷却状态...");
        isCooldown = true;

        yield return new WaitForSeconds(escapeCooldown);

        Debug.Log("冷却结束，可以重新牵制玩家。");
        isCooldown = false;
        restrainCoroutine = null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Root : NetworkBehaviour
{
    public float detectionRange = 10f;
    private EnemyBaseState currentStateBehavior;
    private Enemy enemy;

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

    private void OnDrawGizmosSelected()
    {
        Vector3 rayStartPosition = transform.position + Vector3.up * 1f;

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
                Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * transform.forward;
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
        if (targetPlayer != null && targetPlayer.tag != "Player")
        {
            targetPlayer = null;
            return false;
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            float distanceToPlayer = toPlayer.magnitude;

            if (distanceToPlayer > detectionRange)
                continue;

            float horizontalAngle = Vector3.Angle(transform.forward, toPlayer);
            if (horizontalAngle > fieldOfViewAngleHorizontal / 2f)
                continue;

            float verticalAngle = Vector3.Angle(transform.forward, toPlayer);
            if (verticalAngle > fieldOfViewAngleVertical / 2f)
                continue;

            // 使用视锥判断玩家是否在视野范围内
            for (float horizontal = -fieldOfViewAngleHorizontal / 2; horizontal <= fieldOfViewAngleHorizontal / 2; horizontal += stepAngle)
            {
                for (float vertical = -fieldOfViewAngleVertical / 2; vertical <= fieldOfViewAngleVertical / 2; vertical += stepAngle)
                {
                    Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * transform.forward;

                    if (Physics.Raycast(transform.position + Vector3.up * 1f, rayDirection, out RaycastHit hit, detectionRange))
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

            if (distanceToPlayer > pullRadius)
            {
                Debug.Log("玩家离开牵制范围，牵制结束。");
                break; // 玩家超出牵制范围
            }

            // 玩家挣脱逻辑
            float angleDelta = Vector3.SignedAngle(toPlayer.normalized, -transform.forward.normalized, Vector3.up);
            accumulatedAngle += Mathf.Abs(angleDelta * Time.deltaTime);

            // 玩家挣脱成功
            if (accumulatedAngle >= breakFreeThreshold)
            {
                Debug.Log("玩家挣脱成功！");
                break;
            }

            // 持续牵制效果（玩家掉san等逻辑留空）
            // TODO: 玩家持续掉SAN逻辑由玩家写入

            yield return null;
        }

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
    }
}
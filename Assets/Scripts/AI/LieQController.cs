using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Quaternion = System.Numerics.Quaternion;

public class LieQController : NetworkBehaviour
{
    public float detectionRange = 10f;
    private Enemy enemy;
    private ChasingEnemy _chasingEnemy;

    public float fieldOfViewAngleHorizontal = 120f;
    public float fieldOfViewAngleVertical = 90f;

    [HideInInspector] public GameObject targetPlayer;
    public float stepAngle = 5f; // 多射线之间的步进角度

    public float pullRadius = 5f;       // 牵制范围：腐根为中心半径
    public float breakFreeThreshold = 1000f; // 挣脱角速度的累计阈值
    public float escapeCooldown = 15f;  // 逃脱后的冷却时间
    public bool isRestraining = false; // 是否正在牵制
    public bool isCooldown = false;   // 是否处于冷却中

    public float jumpingDistance = 5f; // 判断跳跃追逐的距离
    private float _originalSpeed;

    public Coroutine restrainCoroutine = null;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        _chasingEnemy = GetComponent<ChasingEnemy>();
    }

    void Update()
    {
        if (enemy.CurrentState is ChasingState)
        {
            if (_chasingEnemy != null)
            {
                if (Vector3.Distance(enemy.transform.position, _chasingEnemy.targetPlayer.transform.position) >
                    jumpingDistance)
                {
                    if (_chasingEnemy.agent.speed != 0)
                    {
                        _originalSpeed = _chasingEnemy.agent.speed;
                        _chasingEnemy.agent.speed = 0;

                        if (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("BigJump"))
                        {
                            Rpc_Jump();
                        }
                    }
                }
                else
                {
                    _chasingEnemy.agent.speed = _originalSpeed;
                }
            }
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Jump()
    {
        enemy.animator.SetTrigger("Jump");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_GoAlive()
    {
        enemy.animator.SetTrigger("goAlive");
    }
    
    public IEnumerator RestrainPlayer()
    {
        isRestraining = true;
        
        yield return new WaitForSeconds(2f);
        Rpc_GoAlive();

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

    public void StartPatrolling()
    {
        enemy.SwitchState(new PatrollingState());
    }
    
}
using System.Collections;
using UnityEngine;
using Fusion;
using Quaternion = System.Numerics.Quaternion;

public class LieQController : NetworkBehaviour
{
    public float detectionRange = 10f;
    private Enemy enemy;
    private ChasingEnemy _chasingEnemy;

    [HideInInspector] public GameObject targetPlayer;
    public float pullRadius = 5f;       // 牵制范围：腐根为中心半径
    public float breakFreeThreshold = 1000f; // 挣脱角速度的累计阈值
    public float escapeCooldown = 15f;  // 逃脱后的冷却时间
    public bool isRestraining = false; // 是否正在牵制
    public bool isCooldown = false;   // 是否处于冷却中

    public float jumpingDistance = 5f; // 判断跳跃追逐的距离
    public float stamina;
    public float maxStamina = 100f;
    public float jumpStaminaCost = 60f;
    public float staminaRecoveryRate = 1f;
    private float _originalSpeed;

    public Coroutine restrainCoroutine = null;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        _chasingEnemy = GetComponent<ChasingEnemy>();
        
        stamina = maxStamina;
    }

    public override void FixedUpdateNetwork()
{
    if (enemy.CurrentState is ChasingState)
    {
        if (_chasingEnemy != null)
        {
            // Assuming stamina is a new field added to LieQController
            if (Vector3.Distance(enemy.transform.position, _chasingEnemy.targetPlayer.transform.position) >
                jumpingDistance && stamina >= jumpStaminaCost)
            {
                if (_chasingEnemy.agent.speed != 0)
                {
                    _originalSpeed = _chasingEnemy.agent.speed;
                    _chasingEnemy.agent.speed = 0;

                    if (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("BigJump"))
                    {
                        Rpc_Jump();
                        stamina -= jumpStaminaCost; // Deduct stamina when jumping
                    }
                }
            }
            else
            {
                _chasingEnemy.agent.speed = _originalSpeed;
            }
        }
    }
    
    // Gradually recover stamina
    if (stamina < maxStamina)
    {
        stamina += staminaRecoveryRate * Time.fixedDeltaTime;
        stamina = Mathf.Min(stamina, maxStamina);
    }
}
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Jump()
    {
        GetComponent<NetworkMecanimAnimator>().SetTrigger("Jump");
    }

    public void Jump()
{
    StartCoroutine(SmoothJump());
}

private IEnumerator SmoothJump()
{
    Vector3 startPosition = transform.position;
    Vector3 targetPosition;
if (enemy == null)
{
    Debug.LogError("Enemy is null");
    targetPosition = transform.position;
}
else if (_chasingEnemy == null)
{
    Debug.LogError("ChasingEnemy is null");
    targetPosition = transform.position;
}
else if (_chasingEnemy.targetPlayer == null)
{
    Debug.LogError("TargetPlayer is null");
    targetPosition = transform.position;
}
else if (_chasingEnemy.agent == null)
{
    Debug.LogError("NavMeshAgent is null");
    targetPosition = enemy.transform.position +
                     transform.forward *
                     Vector3.Distance(enemy.transform.position, _chasingEnemy.targetPlayer.transform.position);
}
else
{
    targetPosition = enemy.transform.position +
                     transform.forward *
                     (Vector3.Distance(enemy.transform.position, _chasingEnemy.targetPlayer.transform.position) -
                      _chasingEnemy.agent.stoppingDistance);
}

    float duration = Vector3.Distance(startPosition, targetPosition) / 10;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        Debug.Log("Jumping: " + transform.position + " to " + targetPosition + " in " + elapsed + " of " + duration);
        transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.position = targetPosition;
}

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_GoAlive()
    {
        GetComponent<NetworkMecanimAnimator>().SetTrigger("goAlive");
    }
    
    public void GoAlive()
{
    if (enemy.sfxClips != null)
    {
        foreach (var clip in enemy.sfxClips)
        {
            if (clip.label == "Awake")
            {
                AudioManager.instance.PlaySFX(enemy.gameObject, clip.clip);
                break;
            }
        }
    }
    if (targetPlayer != null)
    {
        Vector3 direction = targetPlayer.transform.position - transform.position;
        UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.LookRotation(direction);
        // Smoothly rotate with Slerp
        transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        Debug.Log($"Smoothly rotating towards target: {targetPlayer.name}");
    }
}
    
    public void RestrainPlayer()
    {
        isRestraining = true;
        Rpc_GoAlive();

        float accumulatedAngle = 0f;
        Vector3 centerPosition = transform.position;

        while (isRestraining && targetPlayer != null)
        {
            _chasingEnemy.targetPlayer = targetPlayer;
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
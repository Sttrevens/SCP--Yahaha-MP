using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpotlightBase : NetworkBehaviour
{
    [Header("Spotlight Settings")]
    public float speed = 2f;                      // 移动的速度
    public float pauseDuration = 1f;             // 探照点停顿时间
    public LayerMask playerLayer;                // 玩家层级
    public float followSpeed = 3f;               // 跟随模式下的速度
    public float playerLoseThreshold = 10f;      // 玩家摆脱探照灯的距离
    public float maxSpotLightHorizontalAngle = 180f;
    public float maxSpotLightVerticalAngle = 180f;

    [Header("Patrol Settings")]
    public bool useRandomPatrolPoints = true;    // 是否使用随机点
    public Transform[] fixedPatrolPoints;        // 固定的探照点
    public Vector2 patrolCircleRadius = new Vector2(5, 5); // 随机点生成的圆范围
    
    private IEnemyState currentState;            // 当前的状态

    public Enemy enemy;

    public Transform spotlightCenter;
    public Transform spotlightObject;
    
    public GameObject spotlightColliderObject;
    public Collider spotlightCollider;
    
    public AudioClip spotlightSound;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }
    
    /// <summary>
    /// 初始化探照灯
    /// </summary>
    public override void Spawned()
    {
        spotlightCollider = spotlightColliderObject.GetComponent<Collider>();
            
        if (HasStateAuthority)
        {
            // 默认进入普通探照模式
            ChangeState(new SpotlightNormalState());
        }
    }

    /// <summary>
    /// 状态更新逻辑
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        currentState?.UpdateState(enemy);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newState">要切换到的新状态</param>
    public void ChangeState(IEnemyState newState)
    {
        currentState?.ExitState(enemy);
        currentState = newState;
        currentState?.EnterState(enemy);
    }

    /// <summary>
    /// 检测是否发现玩家
    /// </summary>
    /// <returns>是否发现玩家</returns>
    public bool DetectPlayer()
    {
        return spotlightColliderObject.GetComponent<DetectPlayer>().detectPlayerResult;
        /*Collider[] hits = Physics.OverlapSphere(spotlightCollider.bounds.center, spotlightCollider.bounds.extents.magnitude, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true; // 发现玩家
            }
        }
        return false; */ // 未发现玩家
    }

    /// <summary>
    /// 玩家当前位置（如果被发现）
    /// </summary>
    /// <returns>玩家位置</returns>
    public Vector3 GetPlayerPosition()
    {
        return spotlightColliderObject.GetComponent<DetectPlayer>().player.position;
        /*Collider[] hits = Physics.OverlapSphere(spotlightCollider.bounds.center, spotlightCollider.bounds.extents.magnitude, playerLayer);
        if (hits.Length > 0)
        {
            return hits[0].transform.position;
        }
        return Vector3.zero;*/
    }
}
using LPSurvivalEngine;
using UnityEngine;

public class SpotlightChasePlayerState : EnemyBaseState
{
    private float stateEnterTime;
    
    private SpotlightBase spotlight;
    private GameObject playerObject;

    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        // 进入玩家跟踪状态时的初始化逻辑
        Debug.Log("进入玩家跟随模式");
        stateEnterTime = Time.timeSinceLevelLoad; // 记录状态进入时间
    }

    public override void UpdateState(Enemy enemy)
    {
        spotlight = enemy.GetComponent<SpotlightBase>();
        
        // 如果玩家超出范围，切回普通巡逻
        if (!spotlight.DetectPlayer())
        {
            spotlight.ChangeState(new SpotlightNormalState());
            return;
        }

        Vector3 playerPosition = spotlight.GetPlayerPosition();
        // 追踪玩家方向，但不移动灯的本体
        Vector3 directionToPlayer = (playerPosition - spotlight.spotlightObject.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        spotlight.spotlightObject.rotation = Quaternion.Slerp(spotlight.spotlightObject.transform.rotation, targetRotation, spotlight.followSpeed * Time.deltaTime);

        // 自定义玩家被照逻辑
        OnPlayerDetected(playerPosition);
    }

    protected virtual void OnPlayerDetected(Vector3 playerPosition)
    {
        playerObject = spotlight.spotlightColliderObject.GetComponent<DetectPlayer>().player.gameObject;
        AudioManager.instance.PlaySFX(spotlight.spotlightColliderObject, spotlight.spotlightSound);
        
        float elapsedTime = Time.timeSinceLevelLoad - stateEnterTime; // 从状态进入时间计算经过的时间
        playerObject.GetComponent<HealthSystem>().sanity.Subtract(Mathf.Exp(elapsedTime) * 0.1f * Time.fixedDeltaTime);
        // 玩家被照到时的自定义逻辑
        Debug.Log($"玩家在位置 {playerPosition} 被探照灯追踪！");
    }
}
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
        spotlight = enemy.GetComponent<SpotlightBase>();
        
        AudioManager.instance.PlaySFX(spotlight.spotlightColliderObject, spotlight.spotlightSound);
    }

    public override void UpdateState(Enemy enemy)
    {
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

// 限制水平和垂直旋转角度
Vector3 eulerAngles = targetRotation.eulerAngles;
eulerAngles.y = Mathf.Clamp(eulerAngles.y, -spotlight.maxSpotLightHorizontalAngle / 2, spotlight.maxSpotLightHorizontalAngle / 2);
eulerAngles.x = Mathf.Clamp(eulerAngles.x, -spotlight.maxSpotLightVerticalAngle / 2, spotlight.maxSpotLightVerticalAngle / 2);

targetRotation = Quaternion.Euler(eulerAngles);
spotlight.spotlightObject.rotation = Quaternion.Slerp(spotlight.spotlightObject.transform.rotation, targetRotation, spotlight.followSpeed * Time.deltaTime);

        // 自定义玩家被照逻辑
        OnPlayerDetected(playerPosition);
    }

    protected virtual void OnPlayerDetected(Vector3 playerPosition)
    {
        playerObject = spotlight.spotlightColliderObject.GetComponent<DetectPlayer>().player.gameObject;
  
        float elapsedTime = Time.timeSinceLevelLoad - stateEnterTime; // 从状态进入时间计算经过的时间
        playerObject.GetComponent<HealthSystem>().Rpc_Scared(Mathf.Exp(elapsedTime) * 0.1f * Time.fixedDeltaTime);
        // 玩家被照到时的自定义逻辑
        Debug.Log($"玩家在位置 {playerPosition} 被探照灯追踪！");
    }
}
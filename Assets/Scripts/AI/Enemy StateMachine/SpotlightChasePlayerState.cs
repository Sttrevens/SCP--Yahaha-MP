using UnityEngine;

public class SpotlightChasePlayerState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        // 进入玩家跟踪状态时的初始化逻辑
        Debug.Log("进入玩家跟随模式");
    }

    public override void UpdateState(Enemy enemy)
    {
        SpotlightBase spotlight = enemy.GetComponent<SpotlightBase>();
        Vector3 playerPosition = spotlight.GetPlayerPosition();

        // 如果玩家超出范围，切回普通巡逻
        if (Vector3.Distance(playerPosition, enemy.transform.position) > spotlight.playerLoseThreshold)
        {
            spotlight.ChangeState(new SpotlightNormalState());
            return;
        }

        // 追踪玩家方向，但不移动灯的本体
        Vector3 directionToPlayer = (playerPosition - enemy.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        spotlight.spotlightObject.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, spotlight.followSpeed * Time.deltaTime);

        // 自定义玩家被照逻辑
        OnPlayerDetected(playerPosition);
    }

    protected virtual void OnPlayerDetected(Vector3 playerPosition)
    {
        // 玩家被照到时的自定义逻辑
        Debug.Log($"玩家在位置 {playerPosition} 被探照灯追踪！");
    }
}
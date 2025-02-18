using UnityEngine;

public class SpotlightNormalState : EnemyBaseState
{
    private Vector3[] patrolPoints;
    private int currentPointIndex = 0;
    private float pauseTimer = 0f;

    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        if (enemy == null) return;

        SpotlightBase spotlight = enemy.GetComponent<SpotlightBase>();
        if (!spotlight.useRandomPatrolPoints)
        {
            // 使用固定巡逻点
            patrolPoints = new Vector3[spotlight.fixedPatrolPoints.Length];
            for (int i = 0; i < spotlight.fixedPatrolPoints.Length; i++)
            {
                patrolPoints[i] = spotlight.fixedPatrolPoints[i].position;
            }
        }
    }

    // 普通巡逻光束旋转
    public override void UpdateState(Enemy enemy)
    {
        SpotlightBase spotlight = enemy.GetComponent<SpotlightBase>();
        if (spotlight.DetectPlayer())
        {
            Debug.Log("Player detected. Switching to chase player state.");
            spotlight.ChangeState(new SpotlightChasePlayerState());
            return;
        }

        if (spotlight.useRandomPatrolPoints && patrolPoints == null)
        {
            Debug.Log("Generating random patrol point.");
            patrolPoints = new Vector3[1];
            patrolPoints[0] = GenerateRandomPatrolPoint(spotlight);
        }

        if (patrolPoints.Length == 0) 
        {
            Debug.Log("No patrol points available.");
            return;
        }

        Vector3 targetPoint = patrolPoints[currentPointIndex];
        Debug.Log($"Current patrol target: {targetPoint}");

        // 旋转探照灯至目标点方向，而非移动探照灯本体
        Vector3 direction = (targetPoint - enemy.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        spotlight.spotlightObject.rotation = Quaternion.Slerp(spotlight.spotlightObject.transform.rotation, targetRotation, spotlight.speed * Time.deltaTime);
        Debug.Log("Rotating towards target...");

        // 检测是否完成旋转至当前点方向，然后 "移动" 至下一个点逻辑模拟
        if (Quaternion.Angle(spotlight.spotlightObject.transform.rotation, targetRotation) <= 1f) 
        {
            Debug.Log("Rotation to target complete.");
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= spotlight.pauseDuration)
            {
                Debug.Log("Pause duration complete. Moving to next patrol point.");
                pauseTimer = 0f;
                if (spotlight.useRandomPatrolPoints)
                {
                    Debug.Log("Generating new random patrol point.");
                    patrolPoints[0] = GenerateRandomPatrolPoint(spotlight);
                }
                else
                {
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; // 下一个点
                    Debug.Log($"Switched to next patrol point: {currentPointIndex}");
                }
            }
        }
    }

    /// <summary>
    /// 生成随机的单个巡逻点
    /// </summary>
    private Vector3 GenerateRandomPatrolPoint(SpotlightBase spotlight)
    {
        float randomX = Random.Range(-spotlight.patrolCircleRadius.x, spotlight.patrolCircleRadius.x);
        float randomZ = Random.Range(-spotlight.patrolCircleRadius.y, spotlight.patrolCircleRadius.y);
        return new Vector3(spotlight.spotlightCenter.position.x + randomX, spotlight.spotlightCenter.position.y, spotlight.spotlightCenter.position.z + randomZ);
    }
}
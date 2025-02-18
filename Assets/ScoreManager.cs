using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Random = UnityEngine.Random;

public class ScoreManager : NetworkBehaviour
{
    /// <summary>
    /// 维护每个ConeDetection实例ID对应的分数。
    /// </summary>
    private Dictionary<int, float> cameraScoreMap = new Dictionary<int, float>();

    /// <summary>
    /// 用于记录基于时间的额外加分（只要场景中存在任意LiveCamera，就递增）。
    /// </summary>
    private float timerAccumulatedScore = 0f;

    /// <summary>
    /// 计时器，用于达到随机区间就+1分那一套逻辑。
    /// </summary>
    private float timer = 0f;

    // 你可以根据需要，单独提供总分的访问器：
    private float accumulatedTotalScore
    {
        get
        {
            // 先把所有相机分数累加
            float sumCameras = cameraScoreMap.Values.Sum();
            // 再加上计时分数
            return sumCameras + timerAccumulatedScore;
        }
    }

    [Networked] public int networkedTotalScore { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        networkedTotalScore = (int)accumulatedTotalScore;
    }

    private void Update()
    {
        // 每帧检查是否有LiveCamera，如果有，就对 timerAccumulatedScore + 1
        // 并刷新每个相机的分数到 cameraScoreMap
        UpdateTimerAndCameraScores();
    }

    /// <summary>
    /// 负责：只要场景中存在任何打上LiveCamera标签的相机，每帧增加计时并不定期给额外+1分，
    /// 同时更新 cameraScoreMap，为之后统计总分做好准备。
    /// </summary>
    private void UpdateTimerAndCameraScores()
    {
        bool hasLiveCamera = false;
        
        // 找出所有的 camera，对外是 ConeDetection 脚本
        var allConeDetections = FindObjectsOfType<ConeDetection>();
        // 判断是否有至少一个 LiveCamera
        foreach (var cd in allConeDetections)
        {
            if (cd.gameObject.CompareTag("LiveCamera"))
            {
                hasLiveCamera = true;
                break;
            }
        }

        // 如果有任意一个 LiveCamera，则推进计时器
        if (hasLiveCamera)
        {
            timer += Time.deltaTime;
            if (timer >= Random.Range(4f, 14f))
            {
                timerAccumulatedScore += 1f;
                timer = 0f;
            }
        }
        else
        {
            // 没有 LiveCamera，重置计时器
            timer = 0f;
        }

        // 刷新字典中每个相机的分数
        foreach (var cd in allConeDetections)
        {
            int instanceId = cd.GetInstanceID();
            // 如果该相机不在字典中，先初始化一下(不一定必须)
            if (!cameraScoreMap.ContainsKey(instanceId))
                cameraScoreMap[instanceId] = 0f;

            // 将当前的 accumulatedScore 覆盖或更新到字典
            cameraScoreMap[instanceId] = cd.accumulatedScore;
        }
    }

    /// <summary>
    /// 当相机消失(销毁)前，ConeDetection 调用本方法来把自己最后一次的分数提交到 ScoreManager。
    /// </summary>
    /// <param name="instanceID">相机对象的 GetInstanceID()</param>
    /// <param name="finalScore">该相机在消失时的最终分数</param>
    public void OnCameraDestroyed(int instanceID, float finalScore)
    {
        // 如果没有该ID，就补一下，避免KeyNotFound
        if (!cameraScoreMap.ContainsKey(instanceID))
        {
            cameraScoreMap[instanceID] = 0f;
        }
        // 覆盖到该ID对应的分数。这样对象消失后，分数可留存。
        cameraScoreMap[instanceID] = finalScore;
    }
}
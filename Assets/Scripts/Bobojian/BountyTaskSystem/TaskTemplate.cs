using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 任务类型枚举
public enum TaskType
{
    Photography,   // 拍摄类任务
    Survival,      // 生存类任务
    Collection,    // 收集类任务
    Combat,        // 战斗类任务
    Special        // 特殊任务
}

// 任务难度级别
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard,
    VeryHard,
    Extreme
}

// 任务模板（ScriptableObject）
[CreateAssetMenu(fileName = "New Task Template", menuName = "Bounty System/Task Template")]
public class TaskTemplate : ScriptableObject
{
    public string taskID;                // 任务唯一标识
    public string taskName;              // 任务名称
    public string taskDescription;       // 任务描述
    public TaskType taskType;            // 任务类型
    public float baseDifficulty;         // 基础难度系数
    public float baseReward;             // 基础奖励
    
    [TextArea(3, 5)]
    public string[] descriptionFormats;  // 描述文本格式（支持参数化）
    
    // 特定任务类型的附加条件
    public string[] requiredTargetTags;  // 需要的拍摄目标标签
    public bool requiresBoss;            // 是否需要Boss存在
    public bool requiresLowHealth;       // 是否需要玩家低血量
    
    // 任务生成条件
    public float minLevelProgress;       // 最小关卡进度要求
    public float maxLevelProgress;       // 最大关卡进度要求
    public int minViewersRequired;       // 最低需要的观众数
}

// 活跃任务实例
[System.Serializable]
public class ActiveBountyTask
{
    public TaskTemplate template;        // 任务模板
    public string taskId;                // 实例ID
    public string donorName;             // 打赏观众名称
    public float timeLimit;              // 时间限制
    public float remainingTime;          // 剩余时间
    public float reward;                 // 奖励金额
    public bool isCompleted;             // 是否完成
    public bool isFailed;                // 是否失败
    public Dictionary<string, object> taskParameters; // 任务特定参数
}

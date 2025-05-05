using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string[] taskDescriptions;       // 任务描述
    public float baseDifficulty;         // 基础难度系数
    public float baseReward;             // 基础奖励
    public int baseViewerReward;
    
    [TextArea(3, 5)]
    public string[] descriptionFormats;  // 描述文本格式（支持参数化）
    
    // 特定任务类型的附加条件
    public bool requiresSpecificState;            // 任务类型
    public string specificStateName;       // 特定状态名称
    public float requiredTimeLength;
    public int requiresOtherTargetAmount = 1;
    public string[] requiredTargetTags;  // 需要的拍摄目标标签, 如果requiresOtherTarget为true，则必须要全部满足，否则只需满足一个
    
    // 任务生成条件
    public float minLevelProgress;       // 最小关卡进度要求
    public float maxLevelProgress;       // 最大关卡进度要求
    public int minViewersRequired;       // 最低需要的观众数
}

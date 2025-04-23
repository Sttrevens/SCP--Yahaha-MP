using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BountyTaskInfo
{
    public string taskName;                // 任务名称
    public string[] taskDescriptions;      // 可能的任务描述（随机选择）
    public float baseDifficulty = 1f;      // 基础难度系数
    public float baseReward = 100f;        // 基础奖励金额
    public bool requiresSpecificState;     // 是否需要特定状态（如低血量、狂暴等）
    public string specificStateName;       // 特定状态名称
    [Range(0f, 1f)]
    public float taskWeight = 1f;          // 任务生成权重
    public float minProgressToAppear = 0f; // 最小关卡进度要求（0-1）
    public float maxProgressToAppear = 1f; // 最大关卡进度要求（0-1）
}

public class FilmTarget : MonoBehaviour
{
    [Header("美学属性")]
    public float maxAestheticFatigueValue;    // 最大审美疲劳值
    public float currentAestheticFatigueValue; // 当前审美疲劳值
    public int aestheticLevel;                // 拍摄对象的美学等级，越高拍摄得分越高
    public string targetTag;                  // 拍摄对象的标签，方便检索对应的弹幕池
    
    [Header("任务属性")]
    public List<BountyTaskInfo> possibleTasks = new List<BountyTaskInfo>(); // 此目标可能触发的任务列表
    public bool canGenerateTasks = true;      // 是否可以生成赏金任务
    
    [Header("状态追踪")]
    public bool isInSpecialState = false;     // 是否处于特殊状态（如狂暴）
    public string currentState = "";          // 当前状态名称
    
    [Header("事件")]
    // 用于在Unity事件系统中注册状态变化
    public UnityEngine.Events.UnityEvent<string> onStateChanged;
    
    void Start()
    {
        currentAestheticFatigueValue = maxAestheticFatigueValue;
    }
    
    // 更新对象状态（由对象自身逻辑或AI调用）
    public void SetState(string stateName)
    {
        if (currentState != stateName)
        {
            currentState = stateName;
            isInSpecialState = !string.IsNullOrEmpty(stateName);
            onStateChanged?.Invoke(stateName);
        }
    }
    
    // 检查此对象是否符合生成特定任务的条件
    public bool IsEligibleForTaskGeneration(float currentLevelProgress)
    {
        return canGenerateTasks && possibleTasks.Count > 0 && 
               possibleTasks.Exists(task => 
                   currentLevelProgress >= task.minProgressToAppear && 
                   currentLevelProgress <= task.maxProgressToAppear);
    }
    
    // 获取当前可用的任务
    public List<BountyTaskInfo> GetAvailableTasks(float currentLevelProgress)
    {
        List<BountyTaskInfo> availableTasks = new List<BountyTaskInfo>();
        
        foreach (var task in possibleTasks)
        {
            bool progressCondition = currentLevelProgress >= task.minProgressToAppear && 
                                    currentLevelProgress <= task.maxProgressToAppear;
            
            bool stateCondition = !task.requiresSpecificState || 
                                 (isInSpecialState && task.specificStateName == currentState);
            
            if (progressCondition && stateCondition)
            {
                availableTasks.Add(task);
            }
        }
        
        return availableTasks;
    }
}
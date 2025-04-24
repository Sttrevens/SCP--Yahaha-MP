using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BountyTaskInfo
{
    public TaskTemplate task;
    
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
    public string[] allStates;
    
    [Header("事件")]
    // 用于在Unity事件系统中注册状态变化
    public UnityEngine.Events.UnityEvent<string> onStateChanged;

    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }
    
    void Start()
    {
        currentAestheticFatigueValue = maxAestheticFatigueValue;
    }
    
    void Update()
    {
        SyncAnimatorState();
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
    
    public void SyncAnimatorState()
    {
        if (_animator != null && _animator.enabled && allStates != null && allStates.Length > 0)
        {
            // 获取当前激活的动画层
            int layerIndex = 0; // 默认使用第一层
        
            // 获取当前状态信息
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layerIndex);
        
            // 遍历所有已知的状态名称
            foreach (string stateName in allStates)
            {
                // 检查当前是否在这个状态
                if (stateInfo.IsName(stateName))
                {
                    // 找到当前状态，更新状态名并退出循环
                    SetState(stateName);
                    break;
                }
            }
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
            
            bool stateCondition = !task.task.requiresSpecificState || 
                                 (isInSpecialState && task.task.specificStateName == currentState);
            
            if (progressCondition && stateCondition)
            {
                availableTasks.Add(task);
            }
        }
        
        return availableTasks;
    }
    
    public void RemoveTask(BountyTaskInfo task)
    {
        possibleTasks.Remove(task);
    }
}
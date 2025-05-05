using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskStateConditionDetector : BountyTaskManager.ITaskConditionDetector
{
    public bool CheckCondition(FilmTarget target, TaskTemplate taskTemplate)
    {
        if (!taskTemplate.requiresSpecificState)
            return true;
        
        // 检查目标当前状态是否匹配任务要求
        return target.currentState == taskTemplate.specificStateName;
    }
}
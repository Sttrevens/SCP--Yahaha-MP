using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTimeConditionDetector : BountyTaskManager.ITaskConditionDetector
{
    private StreamTaskDetector _detector;
    
    public TaskTimeConditionDetector(StreamTaskDetector detector)
    {
        _detector = detector;
    }
    
    public bool CheckCondition(FilmTarget target, TaskTemplate taskTemplate)
    {
        // 获取目标已经流了多久
        float streamTime = _detector.GetTaskStreamTime(taskTemplate);
        
        // 检查是否达到了任务要求的时长
        return streamTime >= taskTemplate.requiredTimeLength;
    }
}
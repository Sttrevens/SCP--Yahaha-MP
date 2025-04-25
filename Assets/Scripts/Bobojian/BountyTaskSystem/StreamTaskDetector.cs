using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;
using System;

public class StreamTaskDetector : MonoBehaviour
{
    [Header("直播设置")] [SerializeField] private CameraController _cameraController;
    [SerializeField] private float requiredStreamTime = 0.5f; // 需要直播多少秒才算完成

    [Header("检测设置")] [SerializeField] private float checkInterval = 0.5f; // 检测间隔（秒）

    // 跟踪每个目标的直播时间
    private Dictionary<TaskTemplate, float> targetStreamTimes = new Dictionary<TaskTemplate, float>();

    // 跟踪已完成的任务目标
    private HashSet<TaskTemplate> completedTargets = new HashSet<TaskTemplate>();

    // 直播事件（当一个目标被成功直播足够时间）
    public event Action<FilmTarget, float> OnTargetStreamed;

    private float _nextCheckTime;
    private ConeDetection _coneDetection;

    private Dictionary<string, BountyTaskManager.ITaskConditionDetector> _conditionDetectors =
        new Dictionary<string, BountyTaskManager.ITaskConditionDetector>();

    private void Start()
    {
        // 初始化组件引用
        if (_cameraController == null)
        {
            _cameraController = GetComponent<CameraController>();
        }

        if (_cameraController == null)
        {
            Debug.LogError("StreamTaskDetector无法找到CameraController组件！");
            return;
        }

        // 获取或添加ConeDetection组件
        _coneDetection = _cameraController.GetComponent<ConeDetection>();
        if (_coneDetection == null)
        {
            Debug.LogWarning("CameraController没有ConeDetection组件，添加一个");
            _coneDetection = _cameraController.gameObject.AddComponent<ConeDetection>();
        }

        // 设置第一次检测时间
        _nextCheckTime = Time.time + checkInterval;

        InitializeConditionDetectors();
    }

    private void InitializeConditionDetectors()
    {
        // 注册时间检测器
        _conditionDetectors["time"] = new TaskTimeConditionDetector(this);

        // 注册状态检测器
        _conditionDetectors["state"] = new TaskStateConditionDetector();

        // 可以根据需要注册更多检测器...
    }

    private void Update()
    {
        // 检查相机是否有电（如果没电就不计时）
        if (_cameraController.isDied)
        {
            return;
        }

        SyncCurrentTargets();

        // 定期检查而不是每帧检查
        if (Time.time >= _nextCheckTime)
        {
            _nextCheckTime = Time.time + checkInterval;
            UpdateTaskBasedStreamTimes();
        }
    }

    private HashSet<TaskTemplate> currentTasksInView = new HashSet<TaskTemplate>();

    private void SyncCurrentTargets()
    {
        // 先清空当前目标集合
        currentTasksInView.Clear();

// 先收集视野内所有目标的标签
        HashSet<string> allTargetTagsInView = new HashSet<string>();
        Dictionary<string, List<FilmTarget>> tagToTargetsMap = new Dictionary<string, List<FilmTarget>>();
        Dictionary<string, int> tagCountsInView = new Dictionary<string, int>(); // 记录每个标签在视野中出现的次数

        foreach (var obj in _coneDetection.targetsInView)
        {
            var filmTarget = obj.GetComponent<FilmTarget>();
            if (filmTarget != null && !string.IsNullOrEmpty(filmTarget.targetTag))
            {
                allTargetTagsInView.Add(filmTarget.targetTag);

                // 记录标签的出现次数
                if (!tagCountsInView.ContainsKey(filmTarget.targetTag))
                {
                    tagCountsInView[filmTarget.targetTag] = 1;
                }
                else
                {
                    tagCountsInView[filmTarget.targetTag]++;
                }

                // 将标签映射到对应的目标对象
                if (!tagToTargetsMap.ContainsKey(filmTarget.targetTag))
                {
                    tagToTargetsMap[filmTarget.targetTag] = new List<FilmTarget>();
                }

                tagToTargetsMap[filmTarget.targetTag].Add(filmTarget);
            }
        }

// 遍历视野中的每个目标
        foreach (var obj in _coneDetection.targetsInView)
        {
            var filmTarget = obj.GetComponent<FilmTarget>();
            foreach (var taskInfo in filmTarget.possibleTasks)
            {
                TaskTemplate task = taskInfo.task;
                if (task != null)
                {
                    bool shouldAddTask = false;

                    // 检查是否有必需标签
                    if (task.requiredTargetTags == null || task.requiredTargetTags.Length == 0)
                    {
                        // 没有必需标签要求，可以直接添加
                        shouldAddTask = true;
                    }
                    else if (task.requiresOtherTargetAmount <= 1)
                    {
                        // 要求数量小于等于1，按原逻辑处理：只需要当前目标标签匹配任意一个必需标签
                        if (!string.IsNullOrEmpty(filmTarget.targetTag))
                        {
                            foreach (var requiredTag in task.requiredTargetTags)
                            {
                                if (filmTarget.targetTag == requiredTag)
                                {
                                    shouldAddTask = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 要求数量大于1，检查满足标签的数量是否达到要求
                        // 首先计算有多少个不同的requiredTag在视野中
                        int matchedTagsCount = 0;

                        foreach (var requiredTag in task.requiredTargetTags)
                        {
                            if (allTargetTagsInView.Contains(requiredTag))
                            {
                                matchedTagsCount++;
                            }
                        }

                        // 检查满足的标签数量是否达到要求
                        shouldAddTask = matchedTagsCount >= task.requiresOtherTargetAmount;
                    }

                    // 只有满足条件时才添加任务
                    if (shouldAddTask && !currentTasksInView.Contains(task))
                    {
                        currentTasksInView.Add(task);
                    }
                }
            }
        }

        // 移除不再视野内的目标的计时
        List<TaskTemplate> tasksToRemove = new List<TaskTemplate>();
        foreach (var pair in targetStreamTimes)
        {
            if (!currentTasksInView.Contains(pair.Key))
            {
                Debug.Log($"目标 {pair.Key.name} 离开视野,重置计时");
                tasksToRemove.Add(pair.Key);
            }
        }

        foreach (var task in tasksToRemove)
        {
            targetStreamTimes.Remove(task);
        }
    }

// 为每个任务单独跟踪拍摄时间
    private Dictionary<string, float> taskStreamTimes = new Dictionary<string, float>();

// 更新时检查每个活跃任务的目标
    private void UpdateTaskBasedStreamTimes()
    {
        if (BountyTaskManager.Instance == null) return;

        var activeTasks = BountyTaskManager.Instance.GetActiveTasks();
        foreach (var task in activeTasks)
        {
            var currentTask = BountyTaskManager.Instance.GetTaskTemplate(task.id);

            // 如果目标在视野中
            if (currentTasksInView.Contains(currentTask))
            {
                // 初始化计时器（如果不存在）
                if (!taskStreamTimes.ContainsKey(task.id))
                {
                    taskStreamTimes[task.id] = 0;
                    Debug.Log($"开始为任务 {task.taskName} 计时");
                }

                if (_conditionDetectors.ContainsKey("state"))
                {
                    if (_conditionDetectors["state"].CheckCondition(task.targetObject,
                            BountyTaskManager.Instance.GetTaskTemplate(task.id)))
                    {
                        // 增加时间
                        taskStreamTimes[task.id] += checkInterval;
                    }
                }

                // 日志
                Debug.Log($"任务 {task.taskName} 已拍摄 {taskStreamTimes[task.id]:F1} 秒");

                // 检查任务条件
                CheckTaskTimeCompletion(task);
            }
            else
            {
                taskStreamTimes.Remove(task.id);
            }
        }

        // 清理已完成任务的计时器
        List<string> tasksToRemove = new List<string>();
        foreach (var kvp in taskStreamTimes)
        {
            if (!activeTasks.Exists(t => t.id == kvp.Key))
            {
                tasksToRemove.Add(kvp.Key);
            }
        }

        foreach (var taskId in tasksToRemove)
        {
            taskStreamTimes.Remove(taskId);
        }
    }

// 检查特定任务的时间条件
    private void CheckTaskTimeCompletion(BountyTaskManager.ActiveTask task)
    {
        // 获取任务模板
        TaskTemplate taskTemplate = BountyTaskManager.Instance.GetTaskTemplate(task.id);
        if (taskTemplate == null) return;

        // 检查时间是否达到要求
        if (taskStreamTimes.TryGetValue(task.id, out float time) &&
            time >= taskTemplate.requiredTimeLength)
        {
            BountyTaskManager.Instance.CompleteTask(task.id);
            Debug.Log($"任务 {task.taskName} 已完成，拍摄时间: {time:F1}秒");
        }
    }

// 修改 GetTargetStreamTime 方法以支持特定任务
    public float GetTaskStreamTime(TaskTemplate task, string taskId = null)
    {
        // 如果提供了任务ID，则返回该任务的计时
        if (!string.IsNullOrEmpty(taskId) && taskStreamTimes.TryGetValue(taskId, out float taskTime))
        {
            return taskTime;
        }

        // 否则返回目标的通用计时
        if (targetStreamTimes.TryGetValue(task, out float time))
        {
            return time;
        }

        return 0f;
    }

    public List<FilmTarget> GetTargetsInView()
    {
        List<FilmTarget> targetsInView = new List<FilmTarget>();
        foreach (var obj in _coneDetection.targetsInView)
        {
            var filmTarget = obj.GetComponent<FilmTarget>();
            if (filmTarget != null)
            {
                targetsInView.Add(filmTarget);
            }
        }

        return targetsInView;
    }
}
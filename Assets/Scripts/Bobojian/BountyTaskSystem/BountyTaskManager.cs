using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyTaskManager : MonoBehaviour
{
    [System.Serializable]
    public class ActiveTask
    {
        public string id;                     // 任务唯一ID
        public string taskName;               // 任务名称
        public string taskDescription;        // 任务描述
        public FilmTarget targetObject;       // 任务目标
        public string donorName;              // 打赏的观众名称
        public float timeLimit;               // 时间限制（秒）
        public float remainingTime;           // 剩余时间
        public float rewardAmount;            // 奖励金额
        public bool isCompleted;              // 是否已完成
        public bool isFailed;                 // 是否已失败
        public bool isTracking;               // 是否正在追踪
    }
    
    [Header("任务生成设置")]
    public int maxActiveTasks = 3;            // 最大活跃任务数
    public float baseGenerationInterval = 300f; // 基础生成间隔（秒）
    
    [Header("难度设置")]
    public float baseDifficulty = 1f;         // 基础难度
    public float difficultyPerTask = 0.3f;    // 每完成一个任务增加的难度
    public float levelNumFactor = 0.02f;      // 关卡序号系数
    
    [Header("UI元素")]
    public GameObject taskUIPrefab;           // 任务UI预制体
    public Transform taskUIContainer;         // 任务UI容器
    
    [Header("调试信息")]
    public bool showDebugInfo = false;        // 是否显示调试信息
    public string currentLevelName;           // 当前关卡名称
    
    // 私有字段
    private List<ActiveTask> _activeTasks = new List<ActiveTask>();
    private List<string> _recentDonorNames = new List<string>(); // 最近使用的打赏者名称
    private List<FilmTarget> _allTargetsInScene = new List<FilmTarget>();
    private Dictionary<string, List<string>> _usedTaskDescriptions = new Dictionary<string, List<string>>(); // 已使用的任务描述
    private float _nextTaskGenerationTime;
    private int _completedTaskCount = 0;
    private int _failedTaskCount = 0;
    private float _taskSuccessRate = 0.5f; // 默认任务成功率
    private float _levelStartTime;
    private float _estimatedLevelDuration = 300f; // 预计关卡时长（秒）
    
    // 事件
    public delegate void TaskEvent(ActiveTask task);
    public event TaskEvent OnTaskGenerated;
    public event TaskEvent OnTaskCompleted;
    public event TaskEvent OnTaskFailed;
    
    // 单例实例
    public static BountyTaskManager Instance { get; private set; }
    
    [Header("观众互动任务生成")]
    [Tooltip("启用基于观众数量的任务生成")]
    public bool useViewerBasedGeneration = true;
    [Tooltip("累积观众-时间值达到此阈值时生成新任务")]
    public float viewerTimeThreshold = 5000f; // 1000人观看5秒或500人观看10秒都会触发
    [Tooltip("每次检查观众数的时间间隔（秒）")]
    public float viewerCheckInterval = 1f;
    [Tooltip("观众计数乘数（可用于调整生成速度）")]
    public float viewerMultiplier = 1f;
    
    // 新增私有字段
    private float _accumulatedViewerTime = 0f; // 累积的观众-时间值
    private float _nextViewerCheckTime = 0f;   // 下一次检查观众数的时间
    private List<ViewerSnapshot> _recentViewerCounts = new List<ViewerSnapshot>(); // 用于记录观众曲线
    
    // 观众数快照类（用于分析观众趋势）
    private class ViewerSnapshot
    {
        public int viewerCount;
        public float timestamp;
        
        public ViewerSnapshot(int count, float time)
        {
            viewerCount = count;
            timestamp = time;
        }
    }

    
    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // 记录关卡开始时间
        _levelStartTime = Time.time;
        
        // 初始化任务生成时间
        _nextTaskGenerationTime = Time.time + 30f;
    }
    
    private void Start()
    {
        // 查找场景中所有的拍摄目标
        FindAllTargetsInScene();
        
        // 初始化观众名称池
        InitializeDonorNames();
    }
    
    private void Update()
    {
        // 更新所有活跃任务
        UpdateActiveTasks();
        
        // 根据所选机制检查是否应该生成新任务
        if (_activeTasks.Count < maxActiveTasks)
        {
            if (useViewerBasedGeneration)
            {
                CheckViewerBasedTaskGeneration();
            }
            else
            {
                // 原有的基于时间间隔的生成方式
                if (Time.time >= _nextTaskGenerationTime)
                {
                    GenerateTask();
                    _nextTaskGenerationTime = Time.time + GetGenerationInterval();
                }
            }
        }
    }
    
    private void CheckViewerBasedTaskGeneration()
    {
        // 按设定的间隔检查当前观众数
        if (Time.time >= _nextViewerCheckTime)
        {
            // 获取当前观众数
            int currentViewers = ScoreManager.Instance.CurrentViewers;
            
            // 记录观众数快照（可用于分析趋势）
            RecordViewerSnapshot(currentViewers);
            
            // 计算本次间隔累积的观众-时间值
            float intervalViewerTime = currentViewers * viewerCheckInterval * viewerMultiplier;
            
            // 累加到总计数中
            _accumulatedViewerTime += intervalViewerTime;
            
            // 调试信息
            if (showDebugInfo)
            {
                Debug.Log($"当前观众: {currentViewers}, 累积观众-时间值: {_accumulatedViewerTime:F0}/{viewerTimeThreshold:F0}");
            }
            
            // 检查是否达到生成阈值
            if (_accumulatedViewerTime >= viewerTimeThreshold)
            {
                // 生成任务
                GenerateTask();
                
                // 降低累积值（保留部分，避免完全归零造成的节奏断裂）
                float retainedValue = Random.Range(0f, viewerTimeThreshold * 0.2f); // 随机保留0-20%
                _accumulatedViewerTime = retainedValue;
                
                if (showDebugInfo)
                {
                    Debug.Log($"观众互动触发任务生成，保留累积值: {_accumulatedViewerTime:F0}");
                }
            }
            
            // 设置下次检查时间
            _nextViewerCheckTime = Time.time + viewerCheckInterval;
        }
    }
    
    // 记录观众数快照（用于分析趋势）
    private void RecordViewerSnapshot(int viewers)
    {
        _recentViewerCounts.Add(new ViewerSnapshot(viewers, Time.time));
        
        // 只保留最近30秒的记录
        while (_recentViewerCounts.Count > 0 && 
               Time.time - _recentViewerCounts[0].timestamp > 30f)
        {
            _recentViewerCounts.RemoveAt(0);
        }
    }
    
    // 分析观众趋势（上升/下降/稳定）
    private float GetViewerTrend()
    {
        if (_recentViewerCounts.Count < 5) return 0f; // 数据不足
        
        // 取最近5个样本进行线性回归
        int n = Mathf.Min(5, _recentViewerCounts.Count);
        int startIndex = _recentViewerCounts.Count - n;
        
        float sumX = 0f, sumY = 0f, sumXY = 0f, sumXX = 0f;
        float baseTime = _recentViewerCounts[startIndex].timestamp;
        
        for (int i = startIndex; i < _recentViewerCounts.Count; i++)
        {
            float x = _recentViewerCounts[i].timestamp - baseTime; // 相对时间
            float y = _recentViewerCounts[i].viewerCount;
            
            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumXX += x * x;
        }
        
        float slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
        return slope; // 返回斜率，正值表示上升趋势，负值表示下降趋势
    }
    
    // 查找场景中所有拍摄目标
    private void FindAllTargetsInScene()
    {
        _allTargetsInScene.Clear();
        FilmTarget[] targets = FindObjectsOfType<FilmTarget>();
        foreach (var target in targets)
        {
            if (target.canGenerateTasks && target.possibleTasks.Count > 0)
            {
                _allTargetsInScene.Add(target);
                
                // 订阅状态变化事件
                target.onStateChanged.AddListener(state => OnTargetStateChanged(target, state));
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"找到 {_allTargetsInScene.Count} 个可生成任务的目标");
        }
    }
    
    // 当目标状态变化时可能触发特殊任务
    private void OnTargetStateChanged(FilmTarget target, string newState)
    {
        if (_activeTasks.Count >= maxActiveTasks) return;
        
        // 检查是否是特殊状态并有对应的任务
        if (!string.IsNullOrEmpty(newState))
        {
            // 获取当前关卡进度
            float currentProgress = GetLevelProgress();
            
            // 检查此对象是否有与新状态匹配的任务
            var availableTasks = target.GetAvailableTasks(currentProgress);
            var stateTasks = availableTasks.FindAll(t => t.requiresSpecificState && t.specificStateName == newState);
            
            if (stateTasks.Count > 0)
            {
                // 较高概率生成与状态相关的任务
                if (Random.value < 0.7f)
                {
                    GenerateTaskForTarget(target, stateTasks);
                }
            }
        }
    }
    
    // 根据观众数量计算任务生成间隔
    private float GetGenerationInterval()
    {
        int viewerCount = ScoreManager.Instance.CurrentViewers;
        return baseGenerationInterval / (1f + viewerCount);
    }
    
    // 获取当前关卡进度（0-1之间）
    private float GetLevelProgress()
    {
        float elapsedTime = Time.time - _levelStartTime;
        return Mathf.Clamp01(elapsedTime / _estimatedLevelDuration);
    }
    
    // 初始化观众名称池（从弹幕数据中提取）
    private void InitializeDonorNames()
    {
        // 这里应该从弹幕系统获取真实用户名
        // 暂时使用一些示例名称
        _recentDonorNames = new List<string>
        {
            "热心观众", "直播达人", "摄影师", "游戏爱好者", "电影迷", 
            "忠实粉丝", "神秘人", "大佬", "小可爱", "土豪"
        };
    }
    
    // 获取一个随机的打赏者名称
    private string GetRandomDonorName()
    {
        if (_recentDonorNames.Count == 0) return "匿名观众";
        return _recentDonorNames[Random.Range(0, _recentDonorNames.Count)];
    }
    
    // 更新所有活跃任务
    private void UpdateActiveTasks()
    {
        for (int i = _activeTasks.Count - 1; i >= 0; i--)
        {
            var task = _activeTasks[i];
            
            // 更新剩余时间
            task.remainingTime -= Time.deltaTime;
            
            // 检查是否超时
            if (task.remainingTime <= 0 && !task.isCompleted)
            {
                FailTask(task);
                _activeTasks.RemoveAt(i);
            }
        }
    }
    
    private void GenerateTask()
    {
        // 获取观众趋势
        float viewerTrend = GetViewerTrend();
        
        // 计算当前关卡进度
        float currentProgress = GetLevelProgress();
        
        // 查找当前可用的目标
        List<FilmTarget> eligibleTargets = new List<FilmTarget>();
        foreach (var target in _allTargetsInScene)
        {
            if (target.IsEligibleForTaskGeneration(currentProgress))
            {
                eligibleTargets.Add(target);
            }
        }
        
        if (eligibleTargets.Count == 0)
        {
            if (showDebugInfo) Debug.Log("没有合适的目标可以生成任务");
            return;
        }
        
        // 基于权重随机选择一个目标
        FilmTarget selectedTarget = SelectTargetBasedOnWeight(eligibleTargets);
        if (selectedTarget == null) return;
        
        // 获取目标的可用任务并选择一个
        var availableTasks = selectedTarget.GetAvailableTasks(currentProgress);
        if (availableTasks.Count == 0) return;
        
        // 根据观众趋势调整任务参数（例如，观众上升时任务更有价值）
        float trendModifier = 1f + Mathf.Clamp(viewerTrend / 100f, -0.3f, 0.5f);
        
        // 生成并应用趋势修正
        GenerateTaskForTarget(selectedTarget, availableTasks, trendModifier);
    }
    
    // 为特定目标生成任务（增加趋势修正参数）
    private void GenerateTaskForTarget(FilmTarget target, List<BountyTaskInfo> availableTasks, float trendModifier = 1f)
    {
        // 基于权重选择一个任务
        BountyTaskInfo selectedTaskInfo = SelectTaskBasedOnWeight(availableTasks);
        if (selectedTaskInfo == null) return;
        
        // 计算难度
        float totalDifficulty = CalculateTotalDifficulty(selectedTaskInfo.baseDifficulty);
        
        // 应用玩家表现修正
        totalDifficulty *= GetPlayerPerformanceModifier();
        
        // 计算时间限制（受趋势影响）
        float timeLimit = CalculateTimeLimit(totalDifficulty) * trendModifier;
        
        // 计算奖励（受趋势影响更大）
        float reward = CalculateReward(totalDifficulty, selectedTaskInfo.baseReward) * (trendModifier * 1.2f);
        
        // 随机选择一个任务描述
        string taskDescription = GetUniqueTaskDescription(target.targetTag, selectedTaskInfo);
        
        // 获取一个随机打赏者名称
        string donorName = GetRandomDonorName();
        
        // 创建新任务实例
        ActiveTask newTask = new ActiveTask
        {
            id = System.Guid.NewGuid().ToString(),
            taskName = selectedTaskInfo.taskName,
            taskDescription = taskDescription,
            targetObject = target,
            donorName = donorName,
            timeLimit = timeLimit,
            remainingTime = timeLimit,
            rewardAmount = reward,
            isCompleted = false,
            isFailed = false,
            isTracking = true
        };
        
        // 添加到活跃任务列表
        _activeTasks.Add(newTask);
        
        // 触发任务生成事件
        OnTaskGenerated?.Invoke(newTask);
        
        // 创建任务UI
        CreateTaskUI(newTask);
        
        if (showDebugInfo)
        {
            Debug.Log($"已生成任务: {newTask.taskName} - {newTask.taskDescription} - 时限: {timeLimit:F1}秒 - 奖励: {reward:F0} - 趋势修正: {trendModifier:F2}");
        }
    }

    
    // 选择一个唯一的任务描述（避免重复）
    private string GetUniqueTaskDescription(string targetTag, BountyTaskInfo taskInfo)
    {
        if (taskInfo.taskDescriptions == null || taskInfo.taskDescriptions.Length == 0)
            return taskInfo.taskName;
        
        // 初始化已使用描述列表
        if (!_usedTaskDescriptions.ContainsKey(targetTag))
        {
            _usedTaskDescriptions[targetTag] = new List<string>();
        }
        
        // 查找未使用的描述
        List<string> unusedDescriptions = new List<string>();
        foreach (var desc in taskInfo.taskDescriptions)
        {
            if (!_usedTaskDescriptions[targetTag].Contains(desc))
            {
                unusedDescriptions.Add(desc);
            }
        }
        
        // 如果所有描述都已使用，则重置
        if (unusedDescriptions.Count == 0)
        {
            _usedTaskDescriptions[targetTag].Clear();
            unusedDescriptions.AddRange(taskInfo.taskDescriptions);
        }
        
        // 随机选择一个未使用的描述
        string selectedDesc = unusedDescriptions[Random.Range(0, unusedDescriptions.Count)];
        _usedTaskDescriptions[targetTag].Add(selectedDesc);
        
        return selectedDesc;
    }
    
    // 基于权重选择目标
    private FilmTarget SelectTargetBasedOnWeight(List<FilmTarget> targets)
    {
        float totalWeight = 0;
        foreach (var target in targets)
        {
            // 使用美学等级作为权重因子
            totalWeight += target.aestheticLevel;
        }
        
        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;
        
        foreach (var target in targets)
        {
            weightSum += target.aestheticLevel;
            if (randomValue <= weightSum)
            {
                return target;
            }
        }
        
        return targets[0]; // 默认返回第一个
    }
    
    // 基于权重选择任务
    private BountyTaskInfo SelectTaskBasedOnWeight(List<BountyTaskInfo> tasks)
    {
        float totalWeight = 0;
        foreach (var task in tasks)
        {
            totalWeight += task.taskWeight;
        }
        
        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;
        
        foreach (var task in tasks)
        {
            weightSum += task.taskWeight;
            if (randomValue <= weightSum)
            {
                return task;
            }
        }
        
        return tasks[0]; // 默认返回第一个
    }
    
    // 计算总难度
    private float CalculateTotalDifficulty(float baseDifficulty)
    {
        float levelFactor = levelNumFactor * GetCurrentLevelNumber();
        float progressFactor = GetLevelProgress();
        float completionFactor = baseDifficulty + (_completedTaskCount * difficultyPerTask);
        
        return completionFactor + (levelFactor * progressFactor);
    }
    
    // 计算时间限制
    private float CalculateTimeLimit(float totalDifficulty)
    {
        return 90f / (1f + 0.2f * totalDifficulty);
    }
    
    // 计算奖励
    private float CalculateReward(float totalDifficulty, float baseReward)
    {
        int viewerCount = ScoreManager.Instance.CurrentViewers;
        float viewerBonus = 1f + viewerCount / 250f;
        
        return baseReward * totalDifficulty * viewerBonus;
    }
    
    // 根据玩家表现获取难度修正系数
    private float GetPlayerPerformanceModifier()
    {
        if (_taskSuccessRate > 0.7f)
        {
            // 成功率高，难度增加
            return 1.2f;
        }
        else if (_taskSuccessRate < 0.3f)
        {
            // 成功率低，难度降低
            return 0.8f;
        }
        
        return 1.0f; // 正常难度
    }
    
    // 获取当前关卡号
    private int GetCurrentLevelNumber()
    {
        // 这里应实现获取当前关卡编号的逻辑
        return 1; // 示例返回值
    }
    
    // 创建任务UI
    private void CreateTaskUI(ActiveTask task)
    {
        if (taskUIPrefab != null && taskUIContainer != null)
        {
            GameObject taskUIInstance = Instantiate(taskUIPrefab, taskUIContainer);
            TaskUI taskUI = taskUIInstance.GetComponent<TaskUI>();
            if (taskUI != null)
            {
                taskUI.SetupTask(task);
            }
        }
    }
    
    public void CheckStreamTaskCompletion(FilmTarget target)
    {
        foreach (var task in _activeTasks)
        {
            // 如果任务已经完成或失败，跳过
            if (task.isCompleted || task.isFailed) continue;
            
            // 检查任务类型和目标匹配
            if (task.targetObject == target)
            {
                    // 标记任务完成
                    CompleteTask(task.id);
            }
        }
    }
    
    // 完成任务
    public void CompleteTask(string taskId)
    {
        ActiveTask task = _activeTasks.Find(t => t.id == taskId);
        if (task == null || task.isCompleted || task.isFailed)
        {
            Debug.Log("你这任务不对劲");
            return;
        }
        
        task.isCompleted = true;
        
        // 计算最终奖励（考虑剩余时间）
        float remainingTimeRatio = task.remainingTime / task.timeLimit;
        float finalReward = task.rewardAmount * (1f - 0.3f * remainingTimeRatio);
        
        // 给玩家加分
        //ScoreManager.Instance.AddScore((int)finalReward);
        
        // 更新任务统计
        _completedTaskCount++;
        UpdateTaskSuccessRate();
        
        // 触发任务完成事件
        OnTaskCompleted?.Invoke(task);
        
        // 从活跃列表中移除
        _activeTasks.Remove(task);
    }
    
    // 任务失败
    private void FailTask(ActiveTask task)
    {
        if (task.isCompleted || task.isFailed) return;
        
        task.isFailed = true;
        
        // 更新任务统计
        _failedTaskCount++;
        UpdateTaskSuccessRate();
        
        // 触发任务失败事件
        OnTaskFailed?.Invoke(task);
    }
    
    // 更新任务成功率
    private void UpdateTaskSuccessRate()
    {
        int totalTasks = _completedTaskCount + _failedTaskCount;
        if (totalTasks > 0)
        {
            _taskSuccessRate = (float)_completedTaskCount / totalTasks;
        }
    }
    
    // 获取所有活跃任务
    public List<ActiveTask> GetActiveTasks()
    {
        return new List<ActiveTask>(_activeTasks);
    }
}
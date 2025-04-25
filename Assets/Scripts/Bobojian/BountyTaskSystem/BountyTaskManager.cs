using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BountyTaskManager : NetworkBehaviour
{
    [System.Serializable]
    public class ActiveTask
    {
        public string id;                     // 任务唯一ID
        public string taskName;               // 任务名称
        public string taskDescription;        // 任务描述
        public FilmTarget targetObject;       // 任务目标
        public BountyTaskInfo targetTaskInfo;
        public string donorName;              // 打赏的观众名称
        public float timeLimit;               // 时间限制（秒）
        public float remainingTime;           // 剩余时间
        public float rewardAmount;            // 奖励金额
        public int viewerRewardAmount;
    }
    
    public interface ITaskConditionDetector
    {
        bool CheckCondition(FilmTarget target, TaskTemplate taskTemplate);
    }
    
    [Header("任务生成设置")]
    public int maxActiveTasks = 3;            // 最大活跃任务数
    [Tooltip("初始任务的生成时间间隔（秒）。如：[10,20,30]表示游戏开始后10秒生成第一个任务，再过20秒生成第二个，再过30秒生成第三个")]
    public List<float> initialTaskGenerationTimes = new List<float>();
    private int currentInitialTaskIndex = 0;
    private float nextInitialTaskTime = 0f;
    private bool initialTasksCompleted = false;
    
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
    private int _completedTaskCount = 0;
    private int _failedTaskCount = 0;
    private float _taskSuccessRate = 0.5f; // 默认任务成功率
    private bool _isLevelLoaded = false;
    private float _levelStartTime;
    private float _estimatedLevelDuration = 600f; // 预计关卡时长（秒）
    
    // 事件
    public delegate void TaskEvent(ActiveTask task);
    public event TaskEvent OnTaskGenerated;
    public event TaskEvent OnTaskCompleted;
    public event TaskEvent OnTaskFailed;
    
    // 单例实例
    public static BountyTaskManager Instance { get; private set; }
    
    [Header("观众互动任务生成")]
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

    public override void Spawned()
    {
        // 新客户端加入时，如果是服务器，向所有客户端发送当前活跃任务
        if (HasStateAuthority)
        {
            foreach (var task in _activeTasks)
            {
                RPC_BroadcastNewTask(
                    task.id,
                    task.taskName,
                    task.taskDescription,
                    task.targetObject.gameObject.name,
                    task.donorName,
                    task.timeLimit,
                    task.rewardAmount,
                    task.viewerRewardAmount,
                    task.targetObject.targetTag,
                    task.targetTaskInfo.task.taskID
                );
            }
            
            StartCoroutine(DelayedInit());
        
            IEnumerator DelayedInit()
            {
                yield return new WaitForSeconds(15f);
                FindAllTargetsInScene();
            }
        
            // 初始化观众名称池
            InitializeDonorNames();
        
            // 订阅LevelManager的关卡加载和销毁事件
            if (LevelManager.Instance != null)
            {
                // 监听关卡加载完成事件
                LevelManager.Instance.OnLevelLoaded += HandleLevelLoaded;
                // 监听关卡销毁事件
                LevelManager.Instance.OnLevelDestroyed += HandleLevelDestroyed;
            }
            else
            {
                Debug.LogWarning("LevelManager实例不存在，无法绑定关卡事件！");
            }
        }
        else
        {
            // 如果是客户端，向服务器请求当前活跃任务
            RPC_RequestActiveTasks();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestActiveTasks()
    {
        if (!HasStateAuthority) return;
    
        // 服务器收到请求，发送当前所有活跃任务
        foreach (var task in _activeTasks)
        {
            RPC_BroadcastNewTask(
                task.id,
                task.taskName,
                task.taskDescription,
                task.targetObject.gameObject.name,
                task.donorName,
                task.timeLimit,
                task.rewardAmount,
                task.viewerRewardAmount,
                task.targetObject.targetTag,
                task.targetTaskInfo.task.taskID
            );
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
        
        SetupInitialTaskTimes();
    }
    
    // 设置初始任务生成时间
    private void SetupInitialTaskTimes()
    {
        initialTasksCompleted = initialTaskGenerationTimes.Count == 0;
        
        if (!initialTasksCompleted && initialTaskGenerationTimes.Count > 0)
        {
            // 设置第一个初始任务的生成时间
            nextInitialTaskTime = Time.time + initialTaskGenerationTimes[0];
            currentInitialTaskIndex = 0;
            
            if (showDebugInfo)
            {
                Debug.Log($"设置初始任务计划：共{initialTaskGenerationTimes.Count}个预定任务，首个任务将在{initialTaskGenerationTimes[0]}秒后生成");
            }
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded -= HandleLevelLoaded;
            LevelManager.Instance.OnLevelDestroyed -= HandleLevelDestroyed;
        }
    }
    
    // 处理关卡加载完成事件
    private void HandleLevelLoaded()
    {
        _isLevelLoaded = true;
        _levelStartTime = Time.time;
        
        if (showDebugInfo)
        {
            Debug.Log($"关卡已加载，任务系统开始计时 - 时间: {_levelStartTime}");
        }
        
        // 重新查找目标（可能新的关卡有新的目标）
        StartCoroutine(DelayedInit());
        
        IEnumerator DelayedInit()
        {
            yield return new WaitForSeconds(1.5f);
            FindAllTargetsInScene();
            ResetInitialTaskQueue();
        }
    }
    
    // 处理关卡销毁事件
    private void HandleLevelDestroyed()
    {
        _isLevelLoaded = false;
        
        // 清理所有当前活跃的任务
        ClearAllActiveTasks();
        
        // 重置累积的观众-时间值
        _accumulatedViewerTime = 0f;
        
        if (showDebugInfo)
        {
            Debug.Log("关卡已销毁，任务系统已重置");
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        // 更新所有活跃任务
        UpdateActiveTasks();
        
        // 根据所选机制检查是否应该生成新任务
        if (_activeTasks.Count < maxActiveTasks)
        {
            if (!initialTasksCompleted)
            {
                // 先处理初始任务队列
                CheckInitialTaskGeneration();
            }
            else
            {
                // 初始任务队列完成后，使用基于观众数量的生成方式
                CheckViewerBasedTaskGeneration();
            }
        }
    }
    
    // 检查是否应该生成初始队列中的任务
    private void CheckInitialTaskGeneration()
    {
        if (Time.time >= nextInitialTaskTime)
        {
            // 生成一个初始任务
            RPC_GenerateTask();
            if (!HasStateAuthority)
            {
                RPC_RequestActiveTasks();
            }
            
            // 更新到下一个初始任务
            currentInitialTaskIndex++;
            
            if (currentInitialTaskIndex < initialTaskGenerationTimes.Count)
            {
                // 还有更多初始任务，设置下一个任务的时间
                nextInitialTaskTime = Time.time + initialTaskGenerationTimes[currentInitialTaskIndex];
                
                if (showDebugInfo)
                {
                    Debug.Log($"初始任务 {currentInitialTaskIndex} 已生成，下一个任务将在 {initialTaskGenerationTimes[currentInitialTaskIndex]} 秒后生成");
                }
            }
            else
            {
                // 所有初始任务都已生成，转到基于观众人数的算法
                initialTasksCompleted = true;
                
                if (showDebugInfo)
                {
                    Debug.Log("所有初始任务已生成完毕，转为基于观众人数的任务生成机制");
                }
                
                // 重置观众累积值，开始新算法
                _accumulatedViewerTime = 0f;
                _nextViewerCheckTime = Time.time;
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
                RPC_GenerateTask();
                if (!HasStateAuthority)
                {
                    RPC_RequestActiveTasks();
                }
                
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
            var stateTasks = availableTasks.FindAll(t => t.task.requiresSpecificState && t.task.specificStateName == newState);
            
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
    
    // 获取当前关卡进度（0-1之间）
    private float GetLevelProgress()
    {
        // 只有在关卡已加载时才计算进度
        if (!_isLevelLoaded)
            return 0f;
            
        float elapsedTime = Time.time - _levelStartTime;
        return Mathf.Clamp01(elapsedTime / _estimatedLevelDuration);
    }
    
    // 清除所有活跃任务
    private void ClearAllActiveTasks()
    {
        // 复制列表以避免在遍历时修改
        List<ActiveTask> tasksCopy = new List<ActiveTask>(_activeTasks);
        
        foreach (var task in tasksCopy)
        {
            OnTaskFailed?.Invoke(task);
            
            // 从活跃列表中移除
            _activeTasks.Remove(task);
        }
        
        // 清空任务UI
        if (taskUIContainer != null)
        {
            foreach (Transform child in taskUIContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    // 重置初始任务队列
    private void ResetInitialTaskQueue()
    {
        initialTasksCompleted = initialTaskGenerationTimes.Count == 0;
        
        if (!initialTasksCompleted && initialTaskGenerationTimes.Count > 0)
        {
            // 设置第一个初始任务的生成时间
            nextInitialTaskTime = Time.time + initialTaskGenerationTimes[0];
            currentInitialTaskIndex = 0;
            
            if (showDebugInfo)
            {
                Debug.Log($"重置初始任务计划：共{initialTaskGenerationTimes.Count}个预定任务，首个任务将在{initialTaskGenerationTimes[0]}秒后生成");
            }
        }
    }

    
    // 初始化观众名称池（从弹幕数据中提取）
    private void InitializeDonorNames()
    {
        // 这里应该从弹幕系统获取真实用户名
        // 暂时使用一些示例名称
        _recentDonorNames = new List<string>
        {
            "热心观众", "打交特攻队", "我要做绫波丽的狗嘻嘻", "龙龙", "屌丝注定无爱", 
            "忠实粉丝", "神秘人", "大佬", "小可爱", "土豪", "张张", "福瑞控"
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
        if (!HasStateAuthority) return;
        
        for (int i = _activeTasks.Count - 1; i >= 0; i--)
        {
            var task = _activeTasks[i];
            
            // 更新剩余时间
            task.remainingTime -= Time.deltaTime;
            
            // 检查是否超时
            if (task.remainingTime <= 0)
            {
                RPC_FailTask(task.id);
                _activeTasks.RemoveAt(i);
            }
        }
    }
    
   [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
private void RPC_GenerateTask()
{
    if (!HasStateAuthority) return;
    
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
    
    // 根据观众趋势调整任务参数
    float trendModifier = 1f + Mathf.Clamp(viewerTrend / 1000f, -0.3f, 0.5f);
    
    // 基于权重选择一个任务
    BountyTaskInfo selectedTaskInfo = SelectTaskBasedOnWeight(availableTasks);
    if (selectedTaskInfo == null) return;
    
    // 计算各项参数
    float totalDifficulty = CalculateTotalDifficulty(selectedTaskInfo.task.baseDifficulty);
    totalDifficulty *= GetPlayerPerformanceModifier();
    float timeLimit = CalculateTimeLimit(totalDifficulty) * trendModifier;
    float reward = CalculateReward(totalDifficulty, selectedTaskInfo.task.baseReward) * (trendModifier * 1.1f);
    int viewerReward = (int)(CalculateReward(totalDifficulty, selectedTaskInfo.task.baseViewerReward) * (trendModifier * 1.1f));
    string taskDescription = GetUniqueTaskDescription(selectedTarget.targetTag, selectedTaskInfo);
    string donorName = GetRandomDonorName();
    
    // 通知所有客户端生成此任务
    RPC_BroadcastNewTask(
        selectedTaskInfo.task.taskID,
        selectedTaskInfo.task.taskName,
        taskDescription,
        selectedTarget.gameObject.name, // 使用目标的名称而不是引用
        donorName,
        timeLimit,
        reward,
        viewerReward,
        selectedTarget.targetTag,
        selectedTaskInfo.task.taskID // 使用taskID传递任务信息
    );
}

[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
private void RPC_BroadcastNewTask(string id, string taskName, string taskDescription, 
                                 string targetObjectName, string donorName, 
                                 float timeLimit, float rewardAmount, int viewerRewardAmount,
                                 string targetTag, string taskTemplateId)
{
    // 查找目标对象
    FilmTarget targetObject = null;
    GameObject targetGameObject = GameObject.Find(targetObjectName);
    if (targetGameObject != null)
    {
        targetObject = targetGameObject.GetComponent<FilmTarget>();
    }
    
    if (targetObject == null)
    {
        Debug.LogError($"找不到目标对象: {targetObjectName}");
        return;
    }
    
    // 查找任务信息
    BountyTaskInfo targetTaskInfo = null;
    foreach (var taskInfo in targetObject.possibleTasks)
    {
        if (taskInfo.task.taskID == taskTemplateId)
        {
            targetTaskInfo = taskInfo;
            break;
        }
    }
    
    if (targetTaskInfo == null)
    {
        Debug.LogError($"在目标 {targetObjectName} 上找不到任务模板: {taskTemplateId}");
        return;
    }
    
    // 创建新任务实例
    ActiveTask newTask = new ActiveTask
    {
        id = id,
        taskName = taskName,
        taskDescription = taskDescription,
        targetObject = targetObject,
        targetTaskInfo = targetTaskInfo,
        donorName = donorName,
        timeLimit = timeLimit,
        remainingTime = timeLimit,
        rewardAmount = rewardAmount,
        viewerRewardAmount = viewerRewardAmount,
    };

    if (_activeTasks.Contains(newTask))
    {
        RPC_GenerateTask();
        return;
    }
    
    _taskTemplates[newTask.id] = targetTaskInfo.task;
    
    // 添加到活跃任务列表
    _activeTasks.Add(newTask);
    
    // 触发任务生成事件
    OnTaskGenerated?.Invoke(newTask);
    
    // 创建任务UI
    CreateTaskUI(newTask);
    
    if (showDebugInfo)
    {
        Debug.Log($"已生成任务: {newTask.taskName} - {newTask.taskDescription} - 时限: {timeLimit:F1}秒 - 奖励: {rewardAmount:F0}");
    }
}
    
    private Dictionary<string, TaskTemplate> _taskTemplates = new Dictionary<string, TaskTemplate>();
    
    // 为特定目标生成任务（增加趋势修正参数）
    private void GenerateTaskForTarget(FilmTarget target, List<BountyTaskInfo> availableTasks, float trendModifier = 1f)
    {
        // 基于权重选择一个任务
        BountyTaskInfo selectedTaskInfo = SelectTaskBasedOnWeight(availableTasks);
        if (selectedTaskInfo == null) return;
        
        // 计算难度
        float totalDifficulty = CalculateTotalDifficulty(selectedTaskInfo.task.baseDifficulty);
        
        // 应用玩家表现修正
        totalDifficulty *= GetPlayerPerformanceModifier();
        
        // 计算时间限制（受趋势影响）
        float timeLimit = CalculateTimeLimit(totalDifficulty) * trendModifier;
        
        // 计算奖励（受趋势影响更大）
        float reward = CalculateReward(totalDifficulty, selectedTaskInfo.task.baseReward) * (trendModifier * 1.1f);
        int viewerReward = (int)(CalculateReward(totalDifficulty, selectedTaskInfo.task.baseViewerReward) * (trendModifier * 1.1f));
        
        // 随机选择一个任务描述
        string taskDescription = GetUniqueTaskDescription(target.targetTag, selectedTaskInfo);
        
        // 获取一个随机打赏者名称
        string donorName = GetRandomDonorName();
        
        // 创建新任务实例
        ActiveTask newTask = new ActiveTask
        {
            id = selectedTaskInfo.task.taskID,
            taskName = selectedTaskInfo.task.taskName,
            taskDescription = taskDescription,
            targetObject = target,
            targetTaskInfo = selectedTaskInfo,
            donorName = donorName,
            timeLimit = timeLimit,
            remainingTime = timeLimit,
            rewardAmount = reward,
            viewerRewardAmount = viewerReward,
        };
        
        _taskTemplates[newTask.id] = selectedTaskInfo.task;
        
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
        if (taskInfo.task.taskDescriptions == null || taskInfo.task.taskDescriptions.Length == 0)
            return taskInfo.task.taskName;
        
        // 初始化已使用描述列表
        if (!_usedTaskDescriptions.ContainsKey(targetTag))
        {
            _usedTaskDescriptions[targetTag] = new List<string>();
        }
        
        // 查找未使用的描述
        List<string> unusedDescriptions = new List<string>();
        foreach (var desc in taskInfo.task.taskDescriptions)
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
            unusedDescriptions.AddRange(taskInfo.task.taskDescriptions);
        }
        
        // 随机选择一个未使用的描述
        string selectedDesc = unusedDescriptions[Random.Range(0, unusedDescriptions.Count)];
        _usedTaskDescriptions[targetTag].Add(selectedDesc);
        
        return selectedDesc;
    }
    
    // 基于权重选择目标
    private FilmTarget SelectTargetBasedOnWeight(List<FilmTarget> targets)
    {
        float currentProgress = GetLevelProgress();
        List<FilmTarget> targetsWithAvailableTasks = new List<FilmTarget>();
        Dictionary<FilmTarget, float> weightMap = new Dictionary<FilmTarget, float>();
    
        float totalWeight = 0;
    
        // 获取所有StreamTaskDetector
        var detectors = FindObjectsOfType<StreamTaskDetector>();
    
        // 只考虑有可用任务的目标
        foreach (var target in targets)
        {
            var availableTasks = target.GetAvailableTasks(currentProgress);
            if (availableTasks.Count > 0)
            {
                targetsWithAvailableTasks.Add(target);
                float weight = target.aestheticLevel + target.currentAestheticFatigueValue;
                
                // 检查目标是否在任何StreamTaskDetector的视野内
                bool isInView = false;
                foreach (var detector in detectors)
                {
                    if (detector.GetTargetsInView().Contains(target))
                    {
                        isInView = true;
                        break;
                    }
                }
                
                // 如果在视野内，权重翻倍
                if (isInView)
                {
                    weight *= 2;
                }
                
                weightMap[target] = weight;
                totalWeight += weight;
            }
        }
    
        // 如果没有目标有可用任务，返回null
        if (targetsWithAvailableTasks.Count == 0)
        {
            if (showDebugInfo) Debug.Log("没有目标有可用任务");
            return null;
        }
    
        // 基于权重随机选择
        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;
    
        foreach (var target in targetsWithAvailableTasks)
        {
            weightSum += weightMap[target];
            if (randomValue <= weightSum)
            {
                if (showDebugInfo) Debug.Log($"选择目标: {target.name}，权重: {weightMap[target]}/{totalWeight}");
                return target;
            }
        }
    
        return targetsWithAvailableTasks[0];
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
        
        return Mathf.Max(1f, (completionFactor + (levelFactor * progressFactor)) * 0.1f);
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
        
        return baseReward * totalDifficulty + viewerBonus;
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
    
    public TaskTemplate GetTaskTemplate(string taskId)
    {
        if (_taskTemplates.TryGetValue(taskId, out TaskTemplate template))
        {
            return template;
        }
        return null;
    }

// 修改CheckStreamTaskCompletion方法
    public void CheckStreamTaskCompletion(FilmTarget target)
    {
        // 这个方法现在交由StreamTaskDetector处理所有条件逻辑
        // 不需要在这里做任何事情，保留为空方法或记录日志
        if (showDebugInfo)
        {
            Debug.Log($"接收到来自{target.name}的任务检查请求");
        }
    }

    
    // 完成任务
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CompleteTask(string taskId)
    {
        if (!HasStateAuthority)
        {
            Debug.LogError("WTF????");
            return;
        }

        ActiveTask task = _activeTasks.Find(t => t.id == taskId);
        if (task == null)
        {
            Debug.Log("你这任务不对劲");
            return;
        }

        // 给玩家加分
        ScoreManager.Instance.AddMoney(task.rewardAmount);
        ScoreManager.Instance.AddCurrentViewers(task.viewerRewardAmount);

        // 通知所有客户端任务已完成
        RPC_TaskCompleted(taskId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_TaskCompleted(string taskId)
    {
        ActiveTask task = _activeTasks.Find(t => t.id == taskId);
        if (task == null) return;
    
        // 更新统计
        _completedTaskCount++;
        UpdateTaskSuccessRate();
    
        // 触发任务完成事件
        OnTaskCompleted?.Invoke(task);
    
        // 从活跃列表中移除
        _activeTasks.Remove(task);
    
        if (task.targetObject != null)
        {
            task.targetObject.RemoveTask(task.targetTaskInfo);
        }
    }
    
    // 任务失败
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_FailTask(string taskId)
    {
        ActiveTask task = _activeTasks.Find(t => t.id == taskId);
        if (task == null) return;
    
        // 更新任务统计
        _failedTaskCount++;
        UpdateTaskSuccessRate();
    
        // 触发任务失败事件
        OnTaskFailed?.Invoke(task);
    
        // 从活跃列表中移除
        _activeTasks.Remove(task);
    
        if (task.targetObject != null)
        {
            task.targetObject.RemoveTask(task.targetTaskInfo);
        }
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
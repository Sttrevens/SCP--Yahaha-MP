using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;
using System;

public class StreamTaskDetector : MonoBehaviour
{
    [Header("直播设置")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private float requiredStreamTime = 3f; // 需要直播多少秒才算完成
    
    [Header("检测设置")]
    [SerializeField] private float checkInterval = 0.5f; // 检测间隔（秒）
    
    // 跟踪每个目标的直播时间
    private Dictionary<FilmTarget, float> targetStreamTimes = new Dictionary<FilmTarget, float>();
    // 跟踪已完成的任务目标
    private HashSet<FilmTarget> completedTargets = new HashSet<FilmTarget>();
    
    // 直播事件（当一个目标被成功直播足够时间）
    public event Action<FilmTarget, float> OnTargetStreamed;
    
    private float _nextCheckTime;
    private ConeDetection _coneDetection;
    
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
    }
    
    private void Update()
    {
        // 检查相机是否有电（如果没电就不计时）
        if (_cameraController.isDied)
        {
            return; 
        }
        
        // 定期检查而不是每帧检查
        if (Time.time >= _nextCheckTime)
        {
            _nextCheckTime = Time.time + checkInterval;
            CheckTargetsInView();
        }
    }
    
    private void CheckTargetsInView()
    {
        // 获取当前视野内的所有目标
        HashSet<FilmTarget> currentTargets = new HashSet<FilmTarget>();
        foreach (var obj in _coneDetection.targetsInView)
        {
            FilmTarget target = obj.GetComponent<FilmTarget>();
            if (target != null)
            {
                currentTargets.Add(target);
                Debug.Log($"目标 {target.name} 在视野内");
            }
        }
        
        // 增加当前视野内所有目标的直播时间
        foreach (var target in currentTargets)
        {
            // 如果这个目标已经完成了任务，跳过
            /*if (completedTargets.Contains(target))
            {
                continue;
            }*/
                
            // 增加直播时间
            if (!targetStreamTimes.ContainsKey(target))
            {
                targetStreamTimes[target] = 0;
            }
            
            targetStreamTimes[target] += checkInterval;
            
            // 检查是否达到所需直播时间
            if (targetStreamTimes[target] >= requiredStreamTime)
            {
                // 触发直播完成事件
                OnTargetStreamed?.Invoke(target, targetStreamTimes[target]);
                
                // 检查任务完成
                CheckTaskCompletion(target);
                
                // 标记为已完成（防止重复触发）
                //completedTargets.Add(target);
            }
        }
        
        // 移除不再视野内的目标的计时
        List<FilmTarget> targetsToRemove = new List<FilmTarget>();
        foreach (var pair in targetStreamTimes)
        {
            if (!currentTargets.Contains(pair.Key))
            {
                Debug.Log($"目标 {pair.Key.name} 离开视野,重置计时");
                targetsToRemove.Add(pair.Key);
            }
        }
        
        foreach (var target in targetsToRemove)
        {
            targetStreamTimes.Remove(target);
        }
    }
    
    // 检查是否完成了相关任务
    private void CheckTaskCompletion(FilmTarget target)
    {
        if (BountyTaskManager.Instance == null) return;
        
        Debug.Log("我去真完成了");
        // 调用任务管理器检查这个目标的任务是否完成
        BountyTaskManager.Instance.CheckStreamTaskCompletion(target);
    }
    
    // 重置特定目标的完成状态（例如当任务重新生成时）
    public void ResetTargetCompletion(FilmTarget target)
    {
        completedTargets.Remove(target);
        targetStreamTimes.Remove(target);
    }
    
    // 重置所有目标的完成状态
    public void ResetAllTargets()
    {
        completedTargets.Clear();
        targetStreamTimes.Clear();
    }
}
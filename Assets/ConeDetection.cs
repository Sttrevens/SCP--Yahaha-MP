using System.Collections.Generic;
using LPSurvivalEngine;
using UnityEngine;
using System.Linq;

public class ConeDetection : MonoBehaviour
{
    [SerializeField]
    private string targetClassName = "TargetObject";  // 目标脚本名称

    // 检测结果信息
    [Header("检测信息")]
    public bool hasTargetInView = false;  // 视锥体内是否有目标对象
    public float visibleRatio = 0f;       // [指标1] 占屏幕比例
    public float centerOffsetDistance = 0f;  // [指标2] 目标中心点到相机视线轴的垂直距离
    public float distanceToCamera = 0f;    // [指标3] 到相机中心点的距离

    [Header("权重设置")]
    [Range(0, 2)]
    public float centerOffsetWeight = 1.0f;  // 中心偏移距离权重
    [Range(0, 2)]
    public float distanceToCameraWeight = 1.0f;  // 到相机的距离权重
    [Range(0, 2)]
    public float visibleRatioWeight = 0.5f;  // 可见比例的权重

    public Camera cam;

    // 评分相关
    public float accumulatedScore = 0f;  // 累积的分数
    public float realtimeScore = 0f;

    // 缓存目标对象列表 目的是减少FindObjectsOfType的调用次数
    public List<GameObject> cachedTargets = new List<GameObject>();
    public List<GameObject> targetsInView = new List<GameObject>();
    private float updateTargetInterval = 0.5f;  // 更新目标列表的时间间隔
    private float nextUpdateTime = 0.5f;
    
    private float updateScoreInterval = 0.1f;
    private float nextCalculateTime = 0.1f;

    // 用于存储多个目标的评分
    public class TargetScore
    {
        public GameObject target;
        public float score;
        public Vector3 position;
    }
    private List<TargetScore> targetScores = new List<TargetScore>();
    private CameraController _cameraController;

    // 添加这些可配置参数
    [Header("视野检测设置")]
    [SerializeField] private LayerMask obstacleLayer; // 用于设置哪些层可以作为遮挡物
    [SerializeField] private int requiredVisiblePoints = 2; // 需要多少个点可见才算可见
    [SerializeField] private float visibilityThreshold = 0.1f; // 可见性判断的误差范围

    void Start()
    {
        _cameraController = GetComponent<CameraController>();
        
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                enabled = false;
                return;
            }
        }
        UpdateTargetsList();
    }

    void FixedUpdate()
    {
        if (_cameraController != null)
        {
            if (_cameraController.isDied)
            {
                return; 
            }
        }
        
        if (Time.time >= nextUpdateTime)
        {
            UpdateTargetsList();
            nextUpdateTime = Time.time + updateTargetInterval;
        }
        
        if (Time.time >= nextCalculateTime)
        {
            ProcessTargets();
            nextCalculateTime = Time.time + updateScoreInterval;
        }
    }

    private void UpdateTargetsList()
    {
        cachedTargets.Clear();
        var targetObjects = FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in targetObjects)
        {
            if (obj.GetType().Name == targetClassName)
            {
                cachedTargets.Add(obj.gameObject);
            }
        }

        if (cachedTargets.Count == 0)
        {
            realtimeScore = 0f;
        }
    }

    private void ProcessTargets()
    {
        targetScores.Clear();
        hasTargetInView = false;
        targetsInView.Clear();

        int nonVisibleTargetCount = 0;

        foreach (var target in cachedTargets)
        {
            var targetScore = ProcessSingleTarget(target);
            if (targetScore != null)
            {
                targetScores.Add(targetScore);
                if (targetScore.score > 0)
                {
                    hasTargetInView = true;
                    CalculateTotalScore();
                }
                else
                {
                    nonVisibleTargetCount++;
                }
            }
        }

        if (nonVisibleTargetCount >= cachedTargets.Count)
        {
            realtimeScore = 0;
        }
    }

    private TargetScore BestTarget(TargetScore nonVisibleBestTarget = null)
    {
        if (nonVisibleBestTarget != null)
        {
                targetScores.Remove(nonVisibleBestTarget);
        }
        
        var bestTarget = targetScores[0];
        float maxScore = float.MinValue;
        foreach (var score in targetScores)
        {
            if (score.score > maxScore && score.score > 0)
            {
                maxScore = score.score;
                bestTarget = score;
            }
        }
        return bestTarget;
    }

    private void CalculateTotalScore()
    {
        // 重置当前帧的得分
        realtimeScore = 0f;
    
        // 计算所有可见目标的总分
        if (targetScores.Count > 0)
        {
            foreach (var targetScore in targetScores)
            {
                if (targetScore.score > 0)
                {
                    // 累加每个可见目标的分数
                    realtimeScore += targetScore.score;
                    targetsInView.Add(targetScore.target);
                }
            }
        
            // 将当前帧的总分累加到总积分中
            if (realtimeScore > 0)
            {
                accumulatedScore += realtimeScore;
            }
        }
    }

    
    /// <summary>
    /// 处理单个目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private TargetScore ProcessSingleTarget(GameObject target)
    {
        var rends = target.GetComponents<Renderer>().Concat(target.GetComponentsInChildren<Renderer>()).Concat(target.GetComponentsInChildren<SkinnedMeshRenderer>()).ToArray();
        if (rends.Length == 0)
        {
            return null;
        }

var combinedBounds = rends[0].bounds;
foreach (var rend in rends)
{
    combinedBounds.Encapsulate(rend.bounds);
}

var targetScore = new TargetScore
{
    target = target,
    position = combinedBounds.center,
};

        // 视锥体检测
        if (!IsInViewFrustum(combinedBounds))
        {
            return targetScore;
        }

        // 计算可见比例
        float visibilityRatio = CalculateVisibleRatio(combinedBounds);
        if (visibilityRatio <= 0)
        {
            return targetScore;
        }
        
        visibleRatio = visibilityRatio;

        // 计算距离指标
        Vector3 objectCenter = combinedBounds.center;
        distanceToCamera = Vector3.Distance(cam.transform.position, objectCenter);
        centerOffsetDistance = CalculateCenterOffset(objectCenter);

        // 计算得分
        float baseScore = CalculateScore(centerOffsetDistance, distanceToCamera, visibleRatio) * 10;

        if (_cameraController != null)
        {
            var filmTarget = target.GetComponent<FilmTarget>();
            if (filmTarget != null)
            {
                baseScore *= filmTarget.aestheticLevel * (filmTarget.currentAestheticFatigueValue + 1);

                // 随时间流逝，减少 currentAestheticFatigueValue
                filmTarget.currentAestheticFatigueValue =
                    Mathf.Max(0.1f, filmTarget.currentAestheticFatigueValue - Time.fixedDeltaTime);
            }

            if (filmTarget.currentAestheticFatigueValue >= filmTarget.maxAestheticFatigueValue / 3 && visibleRatio >= 0.1f)
            {
                var outLine = target.GetComponent<HasOutLine>();
                if (outLine != null)
                {
                    outLine.SetOutLine(true);
                }
            }
            else
            {
                var outLine = target.GetComponent<HasOutLine>();
                if (outLine != null)
                {
                    outLine.SetOutLine(false);
                }
            }
        }

        // 检查目标状态并调整分数
        var targetBehaviour = target.GetComponent<Enemy>(); // 假设TargetBehaviour脚本包含CurrentState字段
        if (targetBehaviour != null)
        {
            switch (targetBehaviour.CurrentState)
            {
                case ChasingState:
                    baseScore *= 2f; // 为追逐状态调整分数
                    break;
                case AttackingState:
                    baseScore *= 5.0f; // 为攻击状态调整分数
                    break;
                case WaitingforNextAttackState:
                    baseScore *= 3f; // 为等待下次攻击状态调整分数
                    break;
            }
        }

        targetScore.score = baseScore;

        return targetScore;
    }

    private bool IsInViewFrustum(Bounds bounds)
    {
        // 获取视锥体平面
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
    
        // 第一步检查：使用内置视锥体检测
        bool isInFrustum = GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);
    
        if (!isInFrustum)
        {
            return false;
        }

        // 第二步：检查点的可见性
        Vector3[] checkPoints = new Vector3[]
        {
            bounds.center,
            bounds.center + Vector3.up * bounds.extents.y,
            bounds.center - Vector3.up * bounds.extents.y,
            bounds.center + Vector3.right * bounds.extents.x,
            bounds.center - Vector3.right * bounds.extents.x,
            bounds.center + Vector3.forward * bounds.extents.z,
            bounds.center - Vector3.forward * bounds.extents.z
        };

        int visiblePoints = 0;
        foreach (Vector3 point in checkPoints)
        {
            Vector3 directionToPoint = point - cam.transform.position;
            float distanceToPoint = directionToPoint.magnitude;
            Ray ray = new Ray(cam.transform.position, directionToPoint.normalized);
            RaycastHit hit;

            // 使用层遮罩进行射线检测
            if (!Physics.Raycast(ray, out hit, distanceToPoint, obstacleLayer) || 
                Vector3.Distance(hit.point, point) < visibilityThreshold)
            {
                visiblePoints++;
                // Debug.DrawLine(cam.transform.position, point, Color.green, Time.deltaTime);
            }
            else
            {
                // Debug.DrawLine(cam.transform.position, hit.point, Color.red, Time.deltaTime);
            }
        }

        bool enoughVisiblePoints = visiblePoints >= requiredVisiblePoints;

        return enoughVisiblePoints;
    }

    /// <summary>
    /// 计算目标物体在屏幕上的可见比例
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    private float CalculateVisibleRatio(Bounds bounds)
    {
        Vector3[] corners = GetBoundsCorners(bounds);
        Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);
        List<Vector2> screenPoints = new List<Vector2>();

        foreach (var corner in corners)
        {
            // 将目标的角点从世界坐标转换为屏幕坐标
            Vector3 screenPos = cam.WorldToScreenPoint(corner);
            if (screenPos.z > 0)
            {
                Vector2 sp = new Vector2(screenPos.x, screenPos.y);
                screenPoints.Add(sp);
                minPos = Vector2.Min(minPos, sp);
                maxPos = Vector2.Max(maxPos, sp);
            }
        }

        if (screenPoints.Count == 0) return 0f;

        // 计算目标物体在屏幕上的面积
        float objectArea = (maxPos.x - minPos.x) * (maxPos.y - minPos.y);
        // 计算屏幕的面积
        float screenArea = cam.pixelWidth * cam.pixelHeight;
        // 返回可见比例
        return Mathf.Clamp01(objectArea / screenArea);
    }
    /// <summary>
    /// 这个方法计算目标物体中心点到相机视线轴的垂直距离 目标在正前方返回值接近0 在视线的边缘则返回值比较大
    /// </summary>
    /// <param name="objectCenter"></param>
    /// <returns></returns>
    private float CalculateCenterOffset(Vector3 objectCenter)
    {
        Vector3 toObject = objectCenter - cam.transform.position;
        Vector3 forward = cam.transform.forward;
        float distForward = Vector3.Dot(toObject, forward);
        Vector3 projectedPoint = cam.transform.position + forward * distForward;
        return Vector3.Distance(objectCenter, projectedPoint);
    }
    /// <summary>
    /// 计算得分
    /// </summary>
    /// <param name="centerOffset"></param>
    /// <param name="distance"></param>
    /// <param name="visibleRatio"></param>
    /// <returns></returns>
    private float CalculateScore(float centerOffset, float distance, float visibleRatio)
    {
        // 确保中心偏移距离不为0
        float safeCenterOffset = Mathf.Max(centerOffset, 0.0001f);
        // 确保到相机的距离不为0
        float safeDistance = Mathf.Max(distance, 0.0001f);

        return (centerOffsetWeight / safeCenterOffset) +
               (distanceToCameraWeight / safeDistance) +
               (visibleRatioWeight * visibleRatio);
    }

    // ... existing GetBoundsCorners method ...
    /// <summary>
    /// 获取目标的8个角点
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        // 获取目标的中心点和尺寸
        Vector3 center = bounds.center;
        // 获取目标的尺寸
        Vector3 extents = bounds.extents;

        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(+extents.x, +extents.y, +extents.z);
        corners[1] = center + new Vector3(+extents.x, +extents.y, -extents.z);
        corners[2] = center + new Vector3(+extents.x, -extents.y, +extents.z);
        corners[3] = center + new Vector3(+extents.x, -extents.y, -extents.z);
        corners[4] = center + new Vector3(-extents.x, +extents.y, +extents.z);
        corners[5] = center + new Vector3(-extents.x, +extents.y, -extents.z);
        corners[6] = center + new Vector3(-extents.x, -extents.y, +extents.z);
        corners[7] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        return corners;
    }

    private void OnDestroy()
{
    // 在销毁前将当前累积分数回传给 ScoreManager
    var scoreManager = FindObjectOfType<ScoreManager>();
    if (scoreManager != null)
    {
        int myId = GetInstanceID();
        scoreManager.OnCameraDestroyed(myId, accumulatedScore);
    }
}
    
    // 添加到类的最后，在OnDestroy方法后

private void OnDrawGizmos()
{
    if (cam == null)
    {
        if (Camera.main != null)
        {
            cam = Camera.main;
        }
        else
        {
            return;
        }
    }

    // 绘制视锥体
    DrawViewFrustum();
    
    // 可选：绘制射线检测范围指示
    DrawRayVisualization();
}

private void DrawViewFrustum()
{
    // 获取视锥体平面
    Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
    
    // 定义远平面距离 (一般等于摄像机的far clip plane)
    float farDistance = cam.farClipPlane;
    
    // 保存当前Gizmo颜色
    Color originalColor = Gizmos.color;
    
    // 设置视锥体颜色
    Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
    
    // 获取视锥体的8个顶点
    Vector3[] frustumCorners = new Vector3[8];
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
    Vector3[] farCorners = new Vector3[4];
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), farDistance, Camera.MonoOrStereoscopicEye.Mono, farCorners);
    
    // 转换到世界坐标
    for (int i = 0; i < 4; i++)
    {
        frustumCorners[i] = cam.transform.TransformPoint(frustumCorners[i]);
        farCorners[i] = cam.transform.TransformPoint(farCorners[i]);
        frustumCorners[i + 4] = farCorners[i];
    }
    
    // 绘制近平面
    Gizmos.DrawLine(frustumCorners[0], frustumCorners[1]);
    Gizmos.DrawLine(frustumCorners[1], frustumCorners[2]);
    Gizmos.DrawLine(frustumCorners[2], frustumCorners[3]);
    Gizmos.DrawLine(frustumCorners[3], frustumCorners[0]);
    
    // 绘制远平面
    Gizmos.DrawLine(frustumCorners[4], frustumCorners[5]);
    Gizmos.DrawLine(frustumCorners[5], frustumCorners[6]);
    Gizmos.DrawLine(frustumCorners[6], frustumCorners[7]);
    Gizmos.DrawLine(frustumCorners[7], frustumCorners[4]);
    
    // 绘制连接线
    for (int i = 0; i < 4; i++)
    {
        Gizmos.DrawLine(frustumCorners[i], frustumCorners[i + 4]);
    }
    
    // 恢复原始颜色
    Gizmos.color = originalColor;
}

private void DrawRayVisualization()
{
    if (cachedTargets.Count == 0)
        return;

    // 模拟检测
    foreach (var target in cachedTargets)
    {
        var rends = target.GetComponents<Renderer>()
            .Concat(target.GetComponentsInChildren<Renderer>())
            .Concat(target.GetComponentsInChildren<SkinnedMeshRenderer>()).ToArray();
        
        if (rends.Length == 0)
            continue;

        var combinedBounds = rends[0].bounds;
        foreach (var rend in rends)
        {
            combinedBounds.Encapsulate(rend.bounds);
        }

        // 显示边界包围盒的世界坐标和大小
        Vector3 center = combinedBounds.center;
        Vector3 size = combinedBounds.size;
        GUI.color = Color.yellow;
        UnityEngine.Debug.DrawLine(center, center + Vector3.up * 2, Color.yellow, Time.deltaTime);
        
        // 检测视锥体测试结果
        bool isInFrustumGeometry = GeometryUtility.TestPlanesAABB(
            GeometryUtility.CalculateFrustumPlanes(cam), combinedBounds);
            
        // 在目标上方显示视锥体检测结果
        if (isInFrustumGeometry)
        {
            Debug.DrawLine(center + Vector3.up * 2.2f, center + Vector3.up * 2.5f, Color.green, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(center + Vector3.up * 2.2f, center + Vector3.up * 2.5f, Color.red, Time.deltaTime);
        }

        // 检测点
        Vector3[] checkPoints = new Vector3[]
        {
            combinedBounds.center,
            combinedBounds.center + Vector3.up * combinedBounds.extents.y,
            combinedBounds.center - Vector3.up * combinedBounds.extents.y,
            combinedBounds.center + Vector3.right * combinedBounds.extents.x,
            combinedBounds.center - Vector3.right * combinedBounds.extents.x,
            combinedBounds.center + Vector3.forward * combinedBounds.extents.z,
            combinedBounds.center - Vector3.forward * combinedBounds.extents.z
        };

        // 计算可见点数量 - 调试
        int visiblePoints = 0;
        
        // 绘制所有可能的射线，不管是否在视锥体内
        Gizmos.color = Color.gray;
        foreach (Vector3 point in checkPoints)
        {
            Vector3 directionToPoint = point - cam.transform.position;
            float distanceToPoint = directionToPoint.magnitude;
            Ray ray = new Ray(cam.transform.position, directionToPoint.normalized);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, distanceToPoint, obstacleLayer) || 
                Vector3.Distance(hit.point, point) < visibilityThreshold)
            {
                // 无障碍物阻挡
                Gizmos.color = Color.green;
                Gizmos.DrawLine(cam.transform.position, point);
                visiblePoints++;
            }
            else
            {
                // 有障碍物阻挡
                Gizmos.color = Color.red;
                Gizmos.DrawLine(cam.transform.position, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }
        }
        
        // 在目标上方标注可见点数量
        string visibleText = $"{visiblePoints}/{checkPoints.Length}";
        if (visiblePoints >= requiredVisiblePoints)
        {
            Debug.DrawLine(center + Vector3.up * 2.6f, center + Vector3.up * 2.9f, Color.green, Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(center + Vector3.up * 2.6f, center + Vector3.up * 2.9f, Color.red, Time.deltaTime);
        }
        
        // 绘制目标边界框
        if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), combinedBounds))
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.grey;
        }
        Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);
    }
}
}
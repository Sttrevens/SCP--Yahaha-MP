using System.Collections.Generic;
using UnityEngine;

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
    [HideInInspector] public float realtimeScore = 0f;

    // 缓存目标对象列表 目的是减少FindObjectsOfType的调用次数
    private List<GameObject> cachedTargets = new List<GameObject>();
    private float updateTargetInterval = 0.5f;  // 更新目标列表的时间间隔
    private float nextUpdateTime = 0f;

    // 用于存储多个目标的评分
    public class TargetScore
    {
        public GameObject target;
        public float score;
        public bool isVisible;
        public Vector3 position;
    }
    private List<TargetScore> targetScores = new List<TargetScore>();

    // 添加这些可配置参数
    [Header("视野检测设置")]
    [SerializeField] private LayerMask obstacleLayer; // 用于设置哪些层可以作为遮挡物
    [SerializeField] private int requiredVisiblePoints = 2; // 需要多少个点可见才算可见
    [SerializeField] private float visibilityThreshold = 0.1f; // 可见性判断的误差范围

    void Start()
    {
        if (cam == null)
        {
            Debug.Log("未找到相机引用！设置为主相机");
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("未找到相机引用！请手动指定或确保场景中有主相机。");
                enabled = false;
                return;
            }
        }
        UpdateTargetsList();
    }

    void FixedUpdate()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateTargetsList();
            nextUpdateTime = Time.time + updateTargetInterval;
        }

        ProcessTargets();
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
    }

    private void ProcessTargets()
    {
        targetScores.Clear();
        hasTargetInView = false;

        foreach (var target in cachedTargets)
        {
            var targetScore = ProcessSingleTarget(target);
            if (targetScore != null)
            {
                targetScores.Add(targetScore);
                if (targetScore.isVisible)
                {
                    hasTargetInView = true;
                }
            }
        }

        // 找出得分最高的目标
        if (targetScores.Count > 0)
        {
            var bestTarget = targetScores[0];
            float maxScore = float.MinValue;
            foreach (var score in targetScores)
            {
                if (score.score > maxScore && score.isVisible)
                {
                    maxScore = score.score;
                    bestTarget = score;
                }
            }

            // 更新最佳目标的信息
            if (bestTarget.isVisible)
            {
                realtimeScore = bestTarget.score;
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
        var rend = target.GetComponentInChildren<Renderer>();
        if (rend == null) return null;

        var bounds = rend.bounds;
        var targetScore = new TargetScore
        {
            target = target,
            position = bounds.center,
            isVisible = false
        };

        // 视锥体检测
        if (!IsInViewFrustum(bounds)) return targetScore;

        // 计算可见比例
        float visibilityRatio = CalculateVisibleRatio(bounds);
        if (visibilityRatio <= 0) return targetScore;

        targetScore.isVisible = true;
        visibleRatio = visibilityRatio;

        // 计算距离指标
        Vector3 objectCenter = bounds.center;
        distanceToCamera = Vector3.Distance(cam.transform.position, objectCenter);
        centerOffsetDistance = CalculateCenterOffset(objectCenter);

        // 计算得分
        targetScore.score = CalculateScore(centerOffsetDistance, distanceToCamera, visibleRatio) * 10;

        return targetScore;
    }

    private bool IsInViewFrustum(Bounds bounds)
    {
        if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds))
        {
            return false;
        }

        Vector3[] checkPoints = new Vector3[]
        {
            bounds.center,
            bounds.center + Vector3.up * bounds.extents.y,
            bounds.center - Vector3.up * bounds.extents.y,
            bounds.center + Vector3.right * bounds.extents.x,
            bounds.center - Vector3.right * bounds.extents.x
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
                
                // 可选：添加调试可视化
                Debug.DrawLine(cam.transform.position, point, Color.green, Time.deltaTime);
            }
            else
            {
                // 可选：添加调试可视化
                Debug.DrawLine(cam.transform.position, hit.point, Color.red, Time.deltaTime);
            }
        }

        return visiblePoints >= requiredVisiblePoints;
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
}
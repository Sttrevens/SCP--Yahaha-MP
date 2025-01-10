using System.Collections.Generic;
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [SerializeField]
    private string targetClassName = "TargetObject";  // 目标脚本名

    // 供外部查看或调试
    [Header("调试信息")]
    public bool hasTargetInView = false;  // 视锥体中是否有目标物体
    public float visibleRatio = 0f;       // [参数1] 占屏幕比例
    public float centerOffsetDistance = 0f;  // [参数2] 物体中心到摄像机中心射线的横向距离
    public float distanceToCamera = 0f;  // [参数3] 物体中心到摄像机的距离

    // 分数计算的权重系数
    [Header("权重设置")]
    public float centerOffsetWeight = 1.0f;  // 横向偏移距离的权重
    public float distanceToCameraWeight = 1.0f;  // 到摄像机的距离权重
    public float visibleRatioWeight = 0.5f;  // 可见比例的权重

    // 摄像机组件
    public Camera cam;

    // 用于计算每秒分数总和
    public float accumulatedScore = 0f;  // 累积的分数
    private float timeAccumulator = 0f;  // 每秒计时器

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("脚本挂载的物体上没有 Camera 组件！");
        }
    }

    void Update()
    {
        if (cam == null) return;

        // 查找所有目标物体
        var targetObjects = FindObjectsOfType<MonoBehaviour>();
        List<GameObject> matchedObjects = new List<GameObject>();
        foreach (var obj in targetObjects)
        {
            if (obj.GetType().Name == targetClassName)
            {
                matchedObjects.Add(obj.gameObject);
            }
        }

        // 默认无目标
        hasTargetInView = false;
        visibleRatio = 0f;
        centerOffsetDistance = 0f;
        distanceToCamera = 0f;

        // 如果没有目标物体，直接退出
        if (matchedObjects.Count == 0) return;

        // 只检测第一个目标物体
        GameObject target = matchedObjects[0];
        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            hasTargetInView = false;
            return;
        }

        Bounds bounds = rend.bounds;

        // 方法A：判断目标是否在视锥体内
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (!GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            hasTargetInView = false;
            return;
        }
        else
        {
            hasTargetInView = true;
        }

        // 计算可见比例 (visibleRatio)
        Vector3[] corners = GetBoundsCorners(bounds);
        Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);
        List<Vector2> screenPoints = new List<Vector2>();

        foreach (var corner in corners)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(corner);
            if (screenPos.z > 0)
            {
                Vector2 sp = new Vector2(screenPos.x, screenPos.y);
                screenPoints.Add(sp);
                minPos = Vector2.Min(minPos, sp);
                maxPos = Vector2.Max(maxPos, sp);
            }
        }

        if (screenPoints.Count == 0)
        {
            hasTargetInView = false;
            return;
        }

        float objectPixelWidth = maxPos.x - minPos.x;
        float objectPixelHeight = maxPos.y - minPos.y;
        float objectArea = objectPixelWidth * objectPixelHeight;

        float screenWidth = cam.pixelWidth;
        float screenHeight = cam.pixelHeight;
        float screenArea = screenWidth * screenHeight;

        visibleRatio = Mathf.Clamp01(objectArea / screenArea);

        // 计算中心偏移距离和距离相机的距离
        Vector3 objectCenter = bounds.center;
        distanceToCamera = Vector3.Distance(cam.transform.position, objectCenter);

        Vector3 toObject = objectCenter - cam.transform.position;
        Vector3 forward = cam.transform.forward;

        float distForward = Vector3.Dot(toObject, forward);
        Vector3 projectedPoint = cam.transform.position + forward * distForward;

        centerOffsetDistance = Vector3.Distance(objectCenter, projectedPoint);

        // 计算动态分数
        float score = CalculateScore(centerOffsetDistance, distanceToCamera, visibleRatio) * 10;

        // 每秒累计一次分数
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator >= 1f)
        {
            // 每秒累加一次
            accumulatedScore += score;
            //Debug.Log("Accumulated Score this second: " + accumulatedScore);
            timeAccumulator = 0f;  // 重置计时器
        }
    }

    // 计算动态分数
    private float CalculateScore(float centerOffsetDistance, float distanceToCamera, float visibleRatio)
    {
        // 基于权重的计算公式
        // 防止除数为零，确保分母不为零
        float safeCenterOffsetDistance = (centerOffsetDistance > 0f) ? centerOffsetDistance : 0.0001f;
        float safeDistanceToCamera = (distanceToCamera > 0f) ? distanceToCamera : 0.0001f;

        // 计算分数
        float score = (centerOffsetWeight * (1f / safeCenterOffsetDistance)) +
                      (distanceToCameraWeight * (1f / safeDistanceToCamera)) +
                      (visibleRatioWeight * visibleRatio);

        return score;
    }

    // 获取包围盒的 8 个角点
    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
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

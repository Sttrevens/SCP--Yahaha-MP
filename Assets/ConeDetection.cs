using System.Collections.Generic;
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    // 需要检测的目标类（替换为你自己想要检测的脚本/类）
    // 假设你想检测场景中挂有 "TargetObject" 脚本的物体
    [SerializeField]
    private string targetClassName = "TargetObject";

    // 供外部查看或调试
    [Header("调试信息")]
    public bool hasTargetInView = false;        // 视锥体中是否有目标物体
    public float visibleRatio = 0f;             // [参数1] 占屏幕比例
    public float centerOffsetDistance = 0f;     // [参数2] 物体中心到摄像机中心射线的横向距离
    public float distanceToCamera = 0f;         // [参数3] 物体中心到摄像机的距离

    // 摄像机组件
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("脚本挂载的物体上没有 Camera 组件！");
        }
    }

    void Update()
    {
        if (cam == null) return;

        // 查找所有具有指定类名的物体（脚本/组件）
        // 注意：如果你的目标类不是脚本，而是一个具体 MonoBehaviour 类，最好使用
        // FindObjectsOfType<TargetObject>() 这样的方式代替
        var targetObjects = FindObjectsOfType<MonoBehaviour>();
        List<GameObject> matchedObjects = new List<GameObject>();
        foreach (var obj in targetObjects)
        {
            // 判断脚本名是否匹配
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

        // 如果场景中没有指定类的物体，直接退出
        if (matchedObjects.Count == 0) return;

        // 这里为了演示，只检测“第一个”找到的目标物体
        // 如果你需要检测多个，可以自行遍历所有匹配物体

        GameObject target = matchedObjects[0];

        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            // 如果没有 Renderer，就无法得到 Bounds，按不在视野内处理
            hasTargetInView = false;
            return;
        }

        // 获取包围盒
        Bounds bounds = rend.bounds;

        // 方法A：通过GeometryUtility判断与相机六个裁剪平面是否相交
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (!GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            // 物体不在视锥体中
            hasTargetInView = false;
            return;
        }
        else
        {
            hasTargetInView = true;
        }

        // -----------------------------
        // 2) 计算 [参数1] 占屏幕的可见比例
        // -----------------------------
        // 将包围盒的 8 个顶点映射到屏幕坐标（pixel）或者视口坐标（0~1），
        // 然后计算顶点中的最小与最大 x,y，从而得到屏幕覆盖的矩形区域
        Vector3[] corners = GetBoundsCorners(bounds);
        Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

        // 记录在屏幕内的顶点，用于计算可见区域
        List<Vector2> screenPoints = new List<Vector2>();

        foreach (var corner in corners)
        {
            // 将世界坐标转成屏幕像素坐标
            Vector3 screenPos = cam.WorldToScreenPoint(corner);

            // 如果在相机前方（z>0），才可能被看到
            if (screenPos.z > 0)
            {
                Vector2 sp = new Vector2(screenPos.x, screenPos.y);
                screenPoints.Add(sp);

                if (sp.x < minPos.x) minPos.x = sp.x;
                if (sp.y < minPos.y) minPos.y = sp.y;
                if (sp.x > maxPos.x) maxPos.x = sp.x;
                if (sp.y > maxPos.y) maxPos.y = sp.y;
            }
        }

        // 如果完全不在屏幕内（顶点都在后面或可见数为0），判定看不到
        if (screenPoints.Count == 0)
        {
            hasTargetInView = false;
            return;
        }

        // 计算屏幕上的包围盒面积 (像素)
        float objectPixelWidth = maxPos.x - minPos.x;
        float objectPixelHeight = maxPos.y - minPos.y;
        float objectArea = objectPixelWidth * objectPixelHeight;

        // 屏幕总像素面积
        float screenWidth = cam.pixelWidth;
        float screenHeight = cam.pixelHeight;
        float screenArea = screenWidth * screenHeight;

        // 得到物体包围盒投影占屏幕的比例
        // （如果需要3D体积精确计算，需要额外的几何运算，这里以2D投影近似）
        visibleRatio = Mathf.Clamp01(objectArea / screenArea);

        // -----------------------------
        // 3) 计算 [参数2] 与 [参数3]
        // -----------------------------
        // 物体中心点
        Vector3 objectCenter = bounds.center;

        // [参数3] 物体中心到相机的世界距离
        distanceToCamera = Vector3.Distance(cam.transform.position, objectCenter);

        // [参数2] 物体中心到相机“正中心射线”（光轴）的横向距离
        // 可以理解为：先算出物体中心在摄像机前方的投影点，然后计算这两个点的距离
        Vector3 toObject = objectCenter - cam.transform.position;
        // 正中心射线是摄像机 forward 方向
        Vector3 forward = cam.transform.forward;

        // 物体在摄像机 forward 上的投影长度
        float distForward = Vector3.Dot(toObject, forward);

        // 相机位置 + forward * 投影长度 = 相机到物体中心在 forward 方向上的“投影点”
        Vector3 projectedPoint = cam.transform.position + forward * distForward;

        // 物体中心与投影点的距离即为横向距离（是否拍正）
        centerOffsetDistance = Vector3.Distance(objectCenter, projectedPoint);
    }

    /// <summary>
    /// 获取包围盒 Bounds 的 8 个角点
    /// </summary>
    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // 计算出 8 个角点（世界坐标）
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

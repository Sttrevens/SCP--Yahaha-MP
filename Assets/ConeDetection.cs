using System.Collections.Generic;
using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [SerializeField]
    private string targetClassName = "TargetObject";  // Ŀ��ű���

    // ���ⲿ�鿴�����
    [Header("������Ϣ")]
    public bool hasTargetInView = false;  // ��׶�����Ƿ���Ŀ������
    public float visibleRatio = 0f;       // [����1] ռ��Ļ����
    public float centerOffsetDistance = 0f;  // [����2] �������ĵ�������������ߵĺ������
    public float distanceToCamera = 0f;  // [����3] �������ĵ�������ľ���

    // ���������Ȩ��ϵ��
    [Header("Ȩ������")]
    public float centerOffsetWeight = 1.0f;  // ����ƫ�ƾ����Ȩ��
    public float distanceToCameraWeight = 1.0f;  // ��������ľ���Ȩ��
    public float visibleRatioWeight = 0.5f;  // �ɼ�������Ȩ��

    // ��������
    public Camera cam;

    // ���ڼ���ÿ������ܺ�
    public float accumulatedScore = 0f;  // �ۻ��ķ���

    [HideInInspector] public float realtimeScore = 0f;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("�ű����ص�������û�� Camera �����");
        }
    }

    void FixedUpdate()
    {
        if (cam == null) return;

        // ��������Ŀ������
        var targetObjects = FindObjectsOfType<MonoBehaviour>();
        List<GameObject> matchedObjects = new List<GameObject>();
        foreach (var obj in targetObjects)
        {
            if (obj.GetType().Name == targetClassName)
            {
                matchedObjects.Add(obj.gameObject);
            }
        }

        // Ĭ����Ŀ��
        hasTargetInView = false;
        visibleRatio = 0f;
        centerOffsetDistance = 0f;
        distanceToCamera = 0f;

        // ���û��Ŀ�����壬ֱ���˳�
        if (matchedObjects.Count == 0) return;

        // ֻ����һ��Ŀ������
        GameObject target = matchedObjects[0];
        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            hasTargetInView = false;
            return;
        }

        Bounds bounds = rend.bounds;

        // ����A���ж�Ŀ���Ƿ�����׶����
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

        // ����ɼ����� (visibleRatio)
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

        // ��������ƫ�ƾ���;�������ľ���
        Vector3 objectCenter = bounds.center;
        distanceToCamera = Vector3.Distance(cam.transform.position, objectCenter);

        Vector3 toObject = objectCenter - cam.transform.position;
        Vector3 forward = cam.transform.forward;

        float distForward = Vector3.Dot(toObject, forward);
        Vector3 projectedPoint = cam.transform.position + forward * distForward;

        centerOffsetDistance = Vector3.Distance(objectCenter, projectedPoint);

        // ���㶯̬����
        realtimeScore = CalculateScore(centerOffsetDistance, distanceToCamera, visibleRatio) * 10;

        accumulatedScore += realtimeScore;
    }

    // 㶯̬
    private float CalculateScore(float centerOffsetDistance, float distanceToCamera, float visibleRatio)
    {
        // ����Ȩ�صļ��㹫ʽ
        // ��ֹ����Ϊ�㣬ȷ����ĸ��Ϊ��
        float safeCenterOffsetDistance = (centerOffsetDistance > 0f) ? centerOffsetDistance : 0.0001f;
        float safeDistanceToCamera = (distanceToCamera > 0f) ? distanceToCamera : 0.0001f;

        // �������
        float score = (centerOffsetWeight * (1f / safeCenterOffsetDistance)) +
                      (distanceToCameraWeight * (1f / safeDistanceToCamera)) +
                      (visibleRatioWeight * visibleRatio);

        return score;
    }

    // ��ȡ��Χ�е� 8 ���ǵ�
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

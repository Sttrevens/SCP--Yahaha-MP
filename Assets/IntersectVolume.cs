using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectVolume : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera; // 相机，默认是主摄像机
    [SerializeField]
    private GameObject targetObject; // 目标物体
    [SerializeField]
    private int sampleCount = 100; // 每个方向采样点数
    private float intersectVolume; // 交集体积

    void Start()
    {
        // 如果没有设置相机，默认使用主摄像机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // 计算视锥体和物体的相交体积
        intersectVolume = CalculateIntersectVolume();
        Debug.Log($"Intersect Volume: {intersectVolume}");
    }

    /// <summary>
    /// 判断点是否在相机视锥体内
    /// </summary>
    private bool IsInFrustum(Camera cam, Vector3 point)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        foreach (Plane plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0) // 点在任意平面外侧
            {
                return false;
            }
        }
        return true; // 点在所有平面的内侧
    }

    /// <summary>
    /// 判断点是否在物体内部
    /// </summary>
    private bool IsInCollider(MeshCollider other, Vector3 center, Vector3 point)
    {
        Vector3 direction = center - point;
        RaycastHit[] hits = Physics.RaycastAll(point, direction);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == other)
            {
                return false; // 点在物体外部
            }
        }
        return true; // 点在物体内部
    }

    /// <summary>
    /// 计算视锥体与物体的相交体积
    /// </summary>
    private float CalculateIntersectVolume()
    {
        if (targetObject == null || targetCamera == null)
        {
            Debug.LogError("Target Object or Camera is not set!");
            return 0f;
        }

        MeshCollider objectCollider = targetObject.GetComponent<MeshCollider>();
        if (objectCollider == null)
        {
            Debug.LogError("Target Object does not have a MeshCollider!");
            return 0f;
        }

        Vector3 objectCenter = objectCollider.bounds.center;
        Matrix4x4 localToWorld = targetObject.transform.localToWorldMatrix;
        Vector3[] vertices = targetObject.GetComponent<MeshFilter>().mesh.vertices;

        float[] x = new float[vertices.Length];
        float[] y = new float[vertices.Length];
        float[] z = new float[vertices.Length];

        // 转换顶点到世界坐标
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = localToWorld.MultiplyPoint3x4(vertices[i]);
            x[i] = worldVertex.x;
            y[i] = worldVertex.y;
            z[i] = worldVertex.z;
        }

        // 包围盒范围
        Array.Sort(x);
        Array.Sort(y);
        Array.Sort(z);

        float xLength = x[x.Length - 1] - x[0];
        float yLength = y[y.Length - 1] - y[0];
        float zLength = z[z.Length - 1] - z[0];

        // 采样步长
        float stepX = xLength / sampleCount;
        float stepY = yLength / sampleCount;
        float stepZ = zLength / sampleCount;

        int intersectInside = 0;

        for (int i = 0; i < sampleCount; i++)
        {
            for (int j = 0; j < sampleCount; j++)
            {
                for (int k = 0; k < sampleCount; k++)
                {
                    Vector3 samplePoint = new Vector3(
                        x[0] + i * stepX,
                        y[0] + j * stepY,
                        z[0] + k * stepZ
                    );

                    bool inFrustum = IsInFrustum(targetCamera, samplePoint);
                    bool inObject = IsInCollider(objectCollider, objectCenter, samplePoint);

                    if (inFrustum && inObject)
                    {
                        intersectInside++;
                    }
                }
            }
        }

        // 计算相交体积
        return (float)intersectInside / (sampleCount * sampleCount * sampleCount) * (xLength * yLength * zLength);
    }
}

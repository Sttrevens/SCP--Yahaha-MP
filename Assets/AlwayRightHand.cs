using System;
using Unity.VisualScripting;
using UnityEngine;

public class AlwayRightHand : MonoBehaviour
{
    // 距离摄像机的距离，根据你的场景调整
    public float distanceFromCamera = 2f;
    // 视口坐标偏移，可以调整确保 IK 点不完全在边缘
    public Vector2 viewportOffset = new Vector2(0.95f, 0.05f);
    // 如果需要额外的旋转偏移，可以在这里设置（欧拉角）
    public Vector3 additionalRotationOffset;

    // 用于存储初始的旋转偏移量
    public Quaternion initialRotationOffset;
    public Quaternion nowOffet;
    
    private Camera _camera;

    void Start()
    {
        if (Camera.main != null)
        {
            _camera = Camera.main;
            // 计算物体原始旋转与摄像机初始旋转之间的偏移量
            // 公式：物体原始旋转 = 摄像机初始旋转 * 初始偏移量
            // 所以初始偏移量 = 摄像机初始旋转的逆 * 物体原始旋转
            initialRotationOffset = Quaternion.Inverse(Camera.main.transform.rotation) * transform.rotation;
            
            // 如果有额外旋转偏移，则将其叠加进去
            // if (additionalRotationOffset != Vector3.zero)
            // {
            //     initialRotationOffset = initialRotationOffset * Quaternion.Euler(additionalRotationOffset);
            // }
            nowOffet = initialRotationOffset *Quaternion.Euler(additionalRotationOffset);
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        nowOffet = initialRotationOffset *Quaternion.Euler(additionalRotationOffset);
    }
#endif
    void Update()
    {if (_camera != null)
        {
            // 更新位置
            Vector3 viewportPos = new Vector3(viewportOffset.x, viewportOffset.y, distanceFromCamera);
            Vector3 worldPos = _camera.ViewportToWorldPoint(viewportPos);
            // var targetLocalPosition = transform.parent.InverseTransformPoint(worldPos);
            Vector3 zero = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, worldPos, ref zero, 0.05f);
            //transform.position = worldPos;
            // 更新旋转：让物体在保持初始旋转偏移的基础上，跟随摄像机旋转
            transform.rotation = _camera.transform.rotation * nowOffet;
        }
        // if (_camera != null)
        // {
        //     // 更新位置
        //     Vector3 viewportPos = new Vector3(viewportOffset.x, viewportOffset.y, distanceFromCamera);
        //     Vector3 worldPos = _camera.ViewportToWorldPoint(viewportPos);
        //     // var targetLocalPosition = transform.parent.InverseTransformPoint(worldPos);
        //     Vector3 zero = Vector3.zero;
        //     // transform.position = Vector3.SmoothDamp(transform.position, worldPos, ref zero, 0.05f);
        //     transform.position = worldPos;
        //     // 更新旋转：让物体在保持初始旋转偏移的基础上，跟随摄像机旋转
        //     // transform.rotation = _camera.transform.rotation * nowOffet;
        // }
    }

    private void OnRenderObject()
    {
        
    }
    
}
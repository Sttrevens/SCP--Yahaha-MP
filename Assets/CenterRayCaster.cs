using UnityEngine;
using Fusion;

public class UpdateCrosshairPositionWithoutRay : NetworkBehaviour
{
    // 距离摄像机中心沿前方的距离
    public float distanceFromCamera = 10f;

    public override void FixedUpdateNetwork()
    {
        // 获取主摄像机
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("找不到主摄像机，请确保场景中有带有 MainCamera 标签的摄像机！");
            return;
        }
        
        // 利用摄像机的 forward 向量计算目标位置
        Vector3 targetPosition = cam.transform.position + cam.transform.forward * distanceFromCamera;
        
        // 更新当前物体的位置
        transform.position = targetPosition;
    }
}
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    public Transform target; // 摄像机跟随的目标
    public float distance = 5f; // 摄像机与目标之间的默认距离
    public float minDistance = 3f; // 最小距离
    public float maxDistance = 10f; // 最大距离
    public float smoothSpeed = 0.125f; // 平滑移动速度

    private Vector3 offset; // 摄像机与目标的偏移

    void Start()
    {
        offset = transform.position - target.position; // 计算初始的偏移
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        // 射线检测是否有物体在摄像机与目标之间
        RaycastHit hit;
        if (Physics.Raycast(target.position, transform.position - target.position, out hit, offset.magnitude))
        {
            // 如果射线检测到物体，调整摄像机位置，确保不穿模
            float clampedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            transform.position = target.position + (transform.position - target.position).normalized * clampedDistance;
        }
        else
        {
            // 如果没有物体阻挡，保持默认的偏移
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }

        transform.LookAt(target); // 始终保持朝向目标
    }
}

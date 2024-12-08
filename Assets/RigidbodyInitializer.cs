using UnityEngine;

public class RigidbodyInitializer : MonoBehaviour
{
    void Start()
    {
        // 确保 Rigidbody 存在并且正确配置
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false; // 设置为非Kinematic，响应物理模拟
        rb.useGravity = true;   // 启用重力

        // 如果物体有 MeshCollider，确保它是 Convex（适用于物理）
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null && !collider.convex)
        {
            collider.convex = true;
        }
    }
}

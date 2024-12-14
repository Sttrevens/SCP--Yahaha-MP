using UnityEngine;

public class StopMovingObject : MonoBehaviour
{
    private Rigidbody rb;
    public float dampingFactor = 10f; // 阻尼系数，可以根据实际情况调整

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.01f) // 速度大于一定阈值，说明物体在移动
        {
            Vector3 dampingForce = -rb.velocity.normalized * rb.velocity.magnitude * dampingFactor;
            rb.AddForce(dampingForce);
        }
    }
}
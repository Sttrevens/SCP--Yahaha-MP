using System.Collections;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;  // 每秒旋转的速度（度/秒）
    private float rotationAmount = -74.0f;  // 目标旋转角度

    private bool isRotating = false;  // 是否正在旋转
    private Quaternion initialRotation;  // 初始旋转
    private Quaternion targetRotation;   // 目标旋转

    void Start()
    {
        // 初始化旋转状态
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(rotationAmount, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
    }

    /// <summary>
    /// 平滑旋转到目标位置
    /// </summary>
    public void StartRotation()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateToTarget(targetRotation));
        }
    }

    /// <summary>
    /// 平滑复位到初始位置
    /// </summary>
    public void ResetRotation()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateToTarget(initialRotation));
        }
    }

    /// <summary>
    /// 平滑旋转到指定目标
    /// </summary>
    /// <param name="target">目标旋转</param>
    IEnumerator RotateToTarget(Quaternion target)
    {
        isRotating = true;

        // 逐帧插值旋转
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
            yield return null;  // 等待下一帧
        }

        // 确保最终旋转完全对齐
        transform.rotation = target;

        isRotating = false; // 旋转完成
    }
}

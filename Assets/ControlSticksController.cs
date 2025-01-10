using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ControlSticksController : MonoBehaviour, IInteractable
{
    public bool ReciveIsFlying;
    [SerializeField] private bool IsPulled = false; // 默认状态
    public UnityEvent OnButtonPressed; // 事件在 IsPulled 为 true 时调用
    public UnityEvent OnButtonReleased; // 事件在 IsPulled 为 false 时调用

    [SerializeField] private float rotationAngle = 30f; // 旋转角度
    [SerializeField] private float rotationSpeed = 100f; // 旋转速度

    private Quaternion initialRotation; // 初始旋转
    public bool isRotating = false;

    void Start()
    {
        // 记录初始旋转状态
        initialRotation = transform.localRotation;
    }

    public string GetInteractText()
    {
        return string.Format("{0}", IsPulled ? "Close the hatch" : "Open the hatch");
    }

    public void OnInteract()
    {   if (!ReciveIsFlying || isRotating) return;
        IsPulled = !IsPulled;

        if (IsPulled)
        {
            StartCoroutine(RotateToAngle(rotationAngle));
            OnButtonPressed?.Invoke();
        }
        else
        {
            StartCoroutine(RotateToAngle(0f)); // 复位到初始角度
            OnButtonReleased?.Invoke();
        }
    }

    IEnumerator RotateToAngle(float targetAngle)
    {
        isRotating = true;

        Quaternion targetRotation = Quaternion.Euler(targetAngle, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null; // 等待下一帧
        }

        transform.localRotation = targetRotation; // 修正到目标角度
        isRotating = false;
    }
    public void UpdateIsFlying(bool value)
    {
        ReciveIsFlying = value; 
    }
}

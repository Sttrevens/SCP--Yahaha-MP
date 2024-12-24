using UnityEngine;
using TMPro;

public class UITextShake : MonoBehaviour
{
    public TextMeshProUGUI uiText; // 引用到需要抖动的UI文字
    public float shakeAmount = 10f; // 抖动的幅度
    public float shakeSpeed = 5f; // 抖动的速度

    private Vector3 originalPosition; // UI文字的原始位置
    private bool isShaking = false; // 是否在抖动

    void Start()
    {
        originalPosition = uiText.rectTransform.localPosition; // 获取原始位置
    }

    void Update()
    {
        // 监听 E 键的按下和松开
        if (Input.GetKey(KeyCode.E)) // 持续按下E键
        {
            if (!isShaking)
            {
                isShaking = true;
            }

            // 计算抖动效果
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            // 应用抖动效果
            uiText.rectTransform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0);
        }
        else if (isShaking) // 松开E键
        {
            isShaking = false;
            // 恢复到原始位置
            uiText.rectTransform.localPosition = originalPosition;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class StaminaUIController : MonoBehaviour
{
    [Header("耐力条 / 体力条 Image（用于监测 fill 值变化）")]
    public Image staminaImage;

    [Header("需要显示或隐藏的 Image 组")]
    public Image[] targetImages;

    [Header("填充值无变化超过此时长后开始淡出")]
    public float checkDelay = 1f;

    [Header("淡入淡出速度(越大变化越快)")]
    public float fadeSpeed = 2f;

    [Header("目标可见时的透明度(1 = 完全不透明)")]
    public float visibleAlpha = 1f;

    [Header("目标隐藏时的透明度(0 = 完全透明)")]
    public float hiddenAlpha = 0f;

    // 上一次记录的填充值
    private float _lastFillAmount;
    // 记录当前是否处于“正在变化”状态
    private bool _isStaminaChanging;
    // 记录检查计时
    private float _checkTimer;

    private void Start()
    {
        // 初始化记录的填充值
        if (staminaImage != null)
        {
            _lastFillAmount = staminaImage.fillAmount;
        }
    }

    private void Update()
    {
        if (staminaImage == null || targetImages == null || targetImages.Length == 0) 
            return;

        // 1. 检查填充值是否发生变化
        if (Mathf.Abs(staminaImage.fillAmount - _lastFillAmount) > 0.0001f)
        {
            // 如果变化大于一定阈值，视为发生变化
            _isStaminaChanging = true;
            _checkTimer = 0f; // 重置计时
            _lastFillAmount = staminaImage.fillAmount;
        }
        else
        {
            // 没有检测到变化，计时器开始累计
            _checkTimer += Time.deltaTime;
            if (_checkTimer >= checkDelay)
            {
                // 超过一定时间没有变化，就认为完成
                _isStaminaChanging = false;
            }
        }

        // 2. 根据是否变化来控制 alpha
        foreach (var img in targetImages)
        {
            if (img == null) continue;

            // 当前颜色
            Color currentColor = img.color;

            // 目标 alpha 值：如果正在变化，则逐渐显示；否则淡出
            float targetAlpha = _isStaminaChanging ? visibleAlpha : hiddenAlpha;

            // 使用 Lerp 让过渡更平滑
            float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            currentColor.a = newAlpha;
            img.color = currentColor;
        }
    }
}
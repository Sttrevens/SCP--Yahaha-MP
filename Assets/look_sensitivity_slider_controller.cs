using UnityEngine;
using UnityEngine.UI;

public class LookSensitivitySlider : MonoBehaviour
{
    [Header("关联的滑动条组件")]
    public Slider sensitivitySlider;

    [Header("摄像机脚本，用于访问 MouseSensitivity 值")]
    public FirstPersonCamera firstPersonCamera;

    [Header("滑动条最小值")]
    public float minSensitivity = 0.5f;

    [Header("滑动条最大值")]
    public float maxSensitivity = 5f;

    private void Start()
    {
        if (sensitivitySlider == null || firstPersonCamera == null)
        {
            Debug.LogWarning("请先在 Inspector 中指定 Slider 和 FirstPersonCamera。");
            return;
        }

        // 设置滑动条的最小值和最大值
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;

        // 将滑动条初始值设为脚本中当前的 MouseSensitivity
        // 注意：如果 MouseSensitivity 超出 [minSensitivity, maxSensitivity] 范围，需要自行 clamp
        sensitivitySlider.value = Mathf.Clamp(firstPersonCamera.MouseSensitivity, minSensitivity, maxSensitivity);

        // 监听滑动条数值变化事件
        sensitivitySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    /// <summary>
    /// 当滑动条的值改变时，将该值赋给摄像机的 MouseSensitivity
    /// </summary>
    /// <param name="value">滑动条的新值</param>
    private void OnSliderValueChanged(float value)
    {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.MouseSensitivity = value;
        }
    }
}
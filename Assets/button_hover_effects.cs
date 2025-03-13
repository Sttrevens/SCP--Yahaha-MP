using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("背景图片（可选）")]
    public Image backgroundImage;

    public Transform textTransform;

    [Header("放大后的缩放")]
    public Vector3 enlargedScale = new Vector3(1.2f, 1.2f, 1.2f);

    [Header("普通缩放")]
    public Vector3 normalScale = Vector3.one;

    [Header("放大速度(越大变化越快)")]
    public float lerpSpeed = 10f;

    // 记录当前目标缩放
    private Vector3 _targetScale;
    private Color _originalColor;

    private void Awake()
    {
        // 初始化
        if (backgroundImage != null)
            backgroundImage.enabled = false; // 初始时隐藏底图

        // 设置初始缩放
        textTransform.localScale = normalScale;
        _targetScale = normalScale;
        _originalColor = textTransform.GetComponent<TextMeshProUGUI>().color;
    }

    private void Update()
    {
        // 使用Lerp平滑过渡缩放
        textTransform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * lerpSpeed);
    }

    // 鼠标移入事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.enabled = true; // 显示底图

        // 切换目标缩放
        _targetScale = enlargedScale;
        textTransform.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    // 鼠标移出事件
    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.enabled = false; // 隐藏底图

        // 切换目标缩放
        _targetScale = normalScale;
        textTransform.GetComponent<TextMeshProUGUI>().color = _originalColor;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrageHUDController : MonoBehaviour
{
    [Header("淡入淡出设置")] [SerializeField] private float fadeInDuration = 0.5f; // 淡入持续时间(秒)
    [SerializeField] private float fadeOutDuration = 0.5f; // 淡出持续时间(秒)

    private RawImage rawImage; // 当前组件的RawImage引用
    private CanvasGroup canvasGroup; // 可选：使用CanvasGroup控制更多UI元素
    private Coroutine currentFadeCoroutine; // 当前正在执行的淡入淡出协程
    private bool isVisible = true; // 当前可见状态
    
    public static BarrageHUDController instance;

    private void Awake()
    {
        instance = this;
        
        // 获取组件引用
        rawImage = GetComponent<RawImage>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 如果没有CanvasGroup但有RawImage，则初始化透明度
        if (canvasGroup == null && rawImage != null)
        {
            Color color = rawImage.color;
            color.a = isVisible ? 1f : 0f;
            rawImage.color = color;
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
        }
    }

    /// <summary>
    /// 设置显示状态，控制淡入或淡出
    /// </summary>
    /// <param name="visible">是否可见</param>
    /// <returns>当前的可见状态</returns>
    public bool SetVisible(bool visible)
    {
        // 如果状态没有改变，则不执行任何操作
        if (isVisible == visible)
            return isVisible;

        isVisible = visible;

        // 停止当前正在执行的淡入淡出协程（如果有）
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        // 启动新的淡入淡出协程
        if (canvasGroup != null)
        {
            currentFadeCoroutine = StartCoroutine(FadeCanvasGroup(canvasGroup, visible ? 1f : 0f,
                visible ? fadeInDuration : fadeOutDuration));
        }
        else if (rawImage != null)
        {
            currentFadeCoroutine =
                StartCoroutine(FadeRawImage(rawImage, visible ? 1f : 0f, visible ? fadeInDuration : fadeOutDuration));
        }

        return isVisible;
    }

    /// <summary>
    /// 获取当前的可见状态
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }

    /// <summary>
    /// CanvasGroup淡入淡出协程
    /// </summary>
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
    {
        float startAlpha = group.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        group.alpha = targetAlpha;
        currentFadeCoroutine = null;
    }

    /// <summary>
    /// RawImage淡入淡出协程
    /// </summary>
    private IEnumerator FadeRawImage(RawImage image, float targetAlpha, float duration)
    {
        // 如果对象被禁用但要显示，需要先启用它
        if (!gameObject.activeInHierarchy && targetAlpha > 0)
        {
            gameObject.SetActive(true);
        }

        Color startColor = image.color;
        float startAlpha = startColor.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            Color newColor = image.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            image.color = newColor;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = targetAlpha;
        image.color = finalColor;
        currentFadeCoroutine = null;

        // 如果完全透明，可以禁用GameObject以提高性能
        if (targetAlpha == 0f && isVisible == false)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 立即设置透明度，不使用淡入淡出效果
    /// </summary>
    public void SetAlphaImmediate(float alpha)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else if (rawImage != null)
        {
            Color color = rawImage.color;
            color.a = alpha;
            rawImage.color = color;
        }

        isVisible = alpha > 0.01f;
    }
}
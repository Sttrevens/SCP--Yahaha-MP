using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image blackScreenImage; // 用于黑屏的 Image 组件
    public TextMeshProUGUI subtitleText; // 用于显示字幕的 Text 组件

    public string subtitle = "May this journey lead to the stars."; // 显示的字幕文本
    public float fadeDuration = 2.0f; // 渐变持续时间
    public float holdSubtitleDuration = 2.0f; // 字幕持续时间

    public ScoreManager scoreManager;

    private void Awake()
    {
        // 确保黑屏和字幕组件已被赋值
        if (blackScreenImage == null || subtitleText == null)
        {
            Debug.LogError("blackScreenImage 或 subtitleText 未正确赋值！");
            return;
        }

        // 设置黑屏和字幕为全透明并不可见
        blackScreenImage.gameObject.SetActive(true); // 确保对象激活（避免直接修改 color 出错）
        blackScreenImage.color = new Color(0, 0, 0, 0); // 设置为全透明

        subtitleText.gameObject.SetActive(true); // 确保对象激活
        subtitleText.text = ""; // 确保没有文本内容
        subtitleText.color = new Color(1, 1, 1, 0); // 设置为全透明
    }
    private void FadeScreen()
    {
        // 初始时黑屏和字幕不可见
        blackScreenImage.gameObject.SetActive(true);
        subtitleText.gameObject.SetActive(true);

        blackScreenImage.color = new Color(0, 0, 0, 0); // 初始透明度为0
        subtitleText.text = ""; // 初始没有字幕
        subtitleText.color = new Color(1, 1, 1, 0); // 初始透明度为0

        // 启动渐变过程
        StartCoroutine(FadeScreenAndShowSubtitle());
    }

    // 渐变黑屏并显示字幕的协程
    private IEnumerator FadeScreenAndShowSubtitle()
    {
        // 逐渐变黑
        yield return StartCoroutine(FadeToBlack());

        // 显示字幕
        if (scoreManager.totalScore == 0)
        {
            subtitleText.text = subtitle;
        }
        else
        {
            subtitleText.text = "Your total viewers: " + scoreManager.totalScore.ToString("F0");
        }
        yield return StartCoroutine(ShowSubtitle());

        // 逐渐恢复画面
        yield return StartCoroutine(FadeFromBlack());

        // 在恢复画面后清空字幕
        subtitleText.text = "";
        scoreManager.totalScore = 0;
    }

    // 渐变到黑屏
    private IEnumerator FadeToBlack()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration); // 从透明到不透明
            blackScreenImage.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackScreenImage.color = new Color(0, 0, 0, 1); // 确保完全黑屏
    }

    // 显示字幕
    private IEnumerator ShowSubtitle()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration); // 字幕渐显
            subtitleText.color = new Color(1, 1, 1, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        subtitleText.color = new Color(1, 1, 1, 1); // 确保字幕完全可见

        // 等待字幕持续时间
        yield return new WaitForSeconds(holdSubtitleDuration);
    }

    // 渐变恢复画面
    private IEnumerator FadeFromBlack()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration); // 从不透明到透明
            blackScreenImage.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackScreenImage.color = new Color(0, 0, 0, 0); // 确保完全透明
    }
}

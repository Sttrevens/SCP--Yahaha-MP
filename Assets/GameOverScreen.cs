using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public Image blackScreenImage; // 用于黑屏的 Image 组件
    public TextMeshProUGUI subtitleText; // 用于显示字幕的 Text 组件

    public int score; // 公共分数变量

    private void Awake()
    {
        //// 确保黑屏和字幕组件已被赋值
        //if (blackScreenImage == null || subtitleText == null)
        //{
        //    Debug.LogError("blackScreenImage 或 subtitleText 未正确赋值！");
        //    return;
        //}

        //// 设置黑屏和字幕为全透明并不可见
        //blackScreenImage.gameObject.SetActive(false); // 初始时禁用黑屏
        //subtitleText.gameObject.SetActive(false); // 初始时禁用字幕
    }

    /// <summary>
    /// 显示黑屏和分数
    /// </summary>
    public void ShowScore()
    {
        // 启用黑屏和字幕
        blackScreenImage.gameObject.SetActive(true);
        subtitleText.gameObject.SetActive(true);

        // 设置黑屏为黑色
        blackScreenImage.color = new Color(0, 0, 0, 1);

        // 设置字幕为白色并显示分数
        subtitleText.text = $"Your Score is {score}";
        subtitleText.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 隐藏黑屏和分数
    /// </summary>
    public void HideScreen()
    {
        // 禁用黑屏和字幕
        blackScreenImage.gameObject.SetActive(false);
        subtitleText.gameObject.SetActive(false);
    }
}

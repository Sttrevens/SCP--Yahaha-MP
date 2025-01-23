using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlideShowController : MonoBehaviour
{
    public List<Sprite> slides; // 存储所有幻灯片图片
    public Image displayImage; // 用于显示图片的 Image 组件

    private int currentSlideIndex = 0; // 当前显示的图片索引

    void Start()
    {
        if (slides.Count > 0 && displayImage != null)
        {
            displayImage.sprite = slides[currentSlideIndex]; // 显示第一张图片
        }
    }

    void Update()
    {
        // 检测鼠标点击或触摸屏幕
        if (Input.GetMouseButtonDown(0)) // 0 表示左键点击
        {
            ShowNextSlide();
        }
    }

    void ShowNextSlide()
    {
        if (slides.Count == 0) return;

        // 切换到下一张图片
        currentSlideIndex = (currentSlideIndex + 1) % slides.Count;
        displayImage.sprite = slides[currentSlideIndex];
    }
}
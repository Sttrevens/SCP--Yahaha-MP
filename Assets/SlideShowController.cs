using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SlideShowController : MonoBehaviour
{
    public List<Sprite> slides; // 存储所有幻灯片图片
    public Image displayImage; // 用于显示图片的 Image 组件
    public Sprite coverImage;  // 初始封面图

    private int currentSlideIndex = 0; // 当前显示的图片索引
    private bool isFirstSlide = true;   // 用于判断是否是第一次展示幻灯片

    [SerializeField] private AudioClip messageSound;
    [SerializeField] private GameObject messageSoundObject;

    void Start()
    {
        if (coverImage != null && displayImage != null)
        {
            displayImage.sprite = coverImage; // 显示封面图
        }

        StartCoroutine(PlayMessageSound());
    }

    private IEnumerator PlayMessageSound()
    {
        while (isFirstSlide)
        {
            AudioManager.Instance.PlaySFX(messageSoundObject, messageSound);
            Debug.Log("PlayMessageSound");
            yield return new WaitForSeconds(12f);
        }
    }

    public void ShowNextSlide()
    {
        if (slides.Count == 0 || displayImage == null) return;

        // 如果是第一次点击，显示幻灯片的第一张
        if (isFirstSlide)
        {
            currentSlideIndex = 0;  // 确保从第一张幻灯片开始
            displayImage.sprite = slides[currentSlideIndex]; // 显示第一张幻灯片
            isFirstSlide = false;  // 设置为非首次状态
            if(messageSoundObject.GetComponent<AudioSource>() != null)
            {
                StopCoroutine(PlayMessageSound());
                messageSoundObject.GetComponent<AudioSource>().Stop();
            }
        }
        else
        {
            // 切换到下一张图片
            currentSlideIndex = (currentSlideIndex + 1) % slides.Count;
            displayImage.sprite = slides[currentSlideIndex];
        }
    }
}
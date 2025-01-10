using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFade : MonoBehaviour
{   
    public Image blackScreenImage; // 用于黑屏的 Image 组件
    public TextMeshProUGUI subtitleText; // 用于显示字幕的 Text 组件
    // Start is called before the first frame update
    void Awake()
    {
        blackScreenImage.gameObject.SetActive(false);
        subtitleText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

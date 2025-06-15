using System;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class LocalizedFontApplier : MonoBehaviour
{
    public LocalizedTmpFont titleFont;  // Inspector 里指向 TitleFont 键
    TMP_Text txt;

    void OnEnable()
    {
        txt = GetComponent<TMP_Text>();
        titleFont.AssetChanged += OnFontReady;     // 语言切换时会再次触发
        OnFontReady(titleFont.LoadAsset());        // 初始化
    }
    void OnDisable() => titleFont.AssetChanged -= OnFontReady;

    void OnFontReady(TMP_FontAsset f) { if (f) txt.font = f; }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class BarrageItem : NetworkBehaviour
{
    [Networked] public string userNameText { get; set; }
    public Text userName;
    [Networked] public string textText { get; set; }
    public Text text;

    // 本地缓存，用于 Authority 端写入
    private string _localUserNameText;
    private string _localTextText;

    // 延迟布局标记 & RectTransform 缓存
    private bool    _layoutDirty = false;
    private RectTransform _rt;
    private RectTransform _parentContentRt;

    public void setData(BarrageItemJson data, string username)
    {
        if (!Object.HasStateAuthority) return;
        _localUserNameText = username + ": ";
        _localTextText     = data.desc;
    }

    public override void Spawned()
    {
        // 缓存
        _rt = GetComponent<RectTransform>();

        // 首次赋值
        userName.text = userNameText;
        text.text     = textText;
        _rt.localScale = Vector3.one;

        // 等待布局重建
        _layoutDirty = true;
    }

    public override void FixedUpdateNetwork()
    {
        // Authority 端把本地数据写入 Networked 属性
        if (Object.HasStateAuthority)
        {
            userNameText = _localUserNameText;
            textText     = _localTextText;
        }

        // 非 Authority 端接收到更新后，标记需要重建
        bool changed = false;
        if (userName.text != userNameText)
        {
            userName.text = userNameText; changed = true;
        }
        if (text.text != textText)
        {
            text.text = textText; changed = true;
        }
        if (changed) _layoutDirty = true;

        _rt.localScale = Vector3.one;
    }

    private void LateUpdate()
    {
        if (!_layoutDirty || _rt == null) return;

        // 1) 确保所有 Canvas 布局脏标记更新完毕  
        Canvas.ForceUpdateCanvases();
        // 2) 重建自己，触发 HorizontalLayoutGroup + ContentSizeFitter
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
        // 3) 同步重建父容器（scroll_rect.content）  
        if (_parentContentRt != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_parentContentRt);

        _layoutDirty = false;
    }

    /// <summary>
    /// 由外部（BarrageUI）在 Spawn/RPC 完成后调用，设置父容器
    /// </summary>
    public void SetParentContent(RectTransform contentRt)
    {
        _parentContentRt = contentRt;
        _rt.SetParent(contentRt, false);
        _rt.anchoredPosition3D = Vector3.zero;
        _rt.localRotation      = Quaternion.identity;
        _rt.localScale         = Vector3.one;
        // 贴上父后，再次触发重建
        _layoutDirty = true;
    }
}
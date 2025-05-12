using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class BarrageItem : NetworkBehaviour
{
    [Networked] public string userNameText { get; set; }
    public Text    userName;
    [Networked] public string textText     { get; set; }
    public Text    text;

    public ScrollViewNevigation scrollViewNevigation;
    public double min;//最小弹幕出现速度
    public double max;//最大弹幕出现速度
    
    private RectTransform    _rt;
    private RectTransform    _parentContent;
    private bool             _waitingForLayout = false;

    // 本地 Authority 端数据
    private string _localUserNameText;
    private string _localTextText;

    public void SetData(BarrageItemJson data, string username)
    {
        if (!Object.HasStateAuthority) return;
        _localUserNameText = username + ": ";
        _localTextText     = data.desc;
    }

    public override void Spawned()
    {
        _rt = GetComponent<RectTransform>();
    }

    public override void FixedUpdateNetwork()
    {
        // 1) Authority 写入网络属性
        if (Object.HasStateAuthority)
        {
            userNameText = _localUserNameText;
            textText     = _localTextText;
        }
        
            bool changed = false;
            if (userName.text != userNameText)
            {
                userName.text = userNameText;
                changed = true;
            }
            if (text.text != textText)
            {
                text.text = textText;
                changed = true;
            }

            // 发现变动就准备下一帧去做布局 + 滚动
            if (changed && !_waitingForLayout)
            {
                _waitingForLayout = true;
                StartCoroutine(DelayedLayoutAndScroll());
            }
    }

    /// <summary>
    /// BarrageUI 在 RPC_OnItemCreated 里，Spawn 后一定要立刻调用：
    ///    item.SetParentContent(scroll_rect.content);
    /// </summary>
    public void SetParentContent(RectTransform content, ScrollViewNevigation _scrollViewNevigation, double _max, double _min)
    {
        scrollViewNevigation = _scrollViewNevigation;
        max = _max;
        min = _min;
        
        _parentContent = content;
        _rt.SetParent(content, false);
        _rt.localScale = Vector3.one;
    }

    private IEnumerator DelayedLayoutAndScroll()
    {
        // 等到当前帧渲染和网络文本同步完成
        yield return new WaitForEndOfFrame();

        // 强制刷新所有 Canvas 布局
        Canvas.ForceUpdateCanvases();

        // 重建这个 Item 的布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);

        // 重建父容器布局（如果有滚动内容需要自适）
        if (_parentContent != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_parentContent);

        // 触发滚动
        // 注意：这里用具体的 scrollViewNevigation 实例
        scrollViewNevigation.Nevigate(_rt, Mathf.Min(0.8f, ((float)min)/2));

        _waitingForLayout = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LPSurvivalEngine
{
public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DetailPanel detailPanel; //物品详情页的脚本 
    private ItemSlot currentslot;
   
    void Start()
    {
        currentslot = GetComponentInParent<ItemSlotUI>().currentslot;
    }
    
    //3. 鼠标悬浮显示和关闭详情页
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        // 调用 Tooltip 显示
        detailPanel.gameObject.SetActive(true);
        detailPanel.ShowDetail(currentslot.item.displayName);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (detailPanel != null)
        {
            detailPanel.gameObject.SetActive(false);
        }
    }
}
}

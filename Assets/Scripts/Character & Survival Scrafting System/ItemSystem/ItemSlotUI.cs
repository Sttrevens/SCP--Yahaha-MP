using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DestroyIt;

namespace LPSurvivalEngine
{
   public class ItemSlotUI : MonoBehaviour
   {
      [Header("ItemInSlot")]
      public Image icon; //物品的贴图
      public TextMeshProUGUI quantityText;  //物品的数量

      
      [HideInInspector]
      public int index;
      [HideInInspector]
      public bool equipped;
      
      [SerializeField]
      public ItemSlot currentslot;


      //1. Slot中物品的设置和清除
      public void Set(ItemSlot slot)
      {
         currentslot = slot;
         icon.gameObject.SetActive(true);
         icon.sprite = currentslot.item.icon;
 

         quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;   
      }
      

      public void Clear()
      {
         currentslot = null;
         icon.gameObject.SetActive(false);
         quantityText.text = string.Empty;
      }
      
   }


}
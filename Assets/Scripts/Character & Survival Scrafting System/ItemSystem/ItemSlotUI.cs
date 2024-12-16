using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace LPSurvivalEngine
{
   public class ItemSlotUI : MonoBehaviour
   {
      [Header("ItemInSlot")]
      public Button ItemSelectbutton; //选择物品的按键
      public Image icon; //物品的贴图
      public TextMeshProUGUI quantityText;  //物品的数量
      
      [Space]
      [Header("Buttons")]
      public GameObject useButton;
      public GameObject equipButton;
      public GameObject dropItemButton;
      public GameObject dropButton;

      [SerializeField]
      public int index;
      public bool equipped;
      
      [SerializeField]
      public ItemSlot currentslot;
      private bool isEmpty = true;

      void Start()
      {
         ItemSelectbutton.onClick.AddListener(OnClickButton);
      }

      //1. Slot中物品的设置和清除
      public void Set(ItemSlot slot)
      {
         currentslot = slot;
         
         isEmpty = false;
         
         icon.gameObject.SetActive(true);
         icon.sprite = slot.item.icon;

         quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;   
      }
      

      public void Clear()
      {
         currentslot = null;
         icon.gameObject.SetActive(false);
         quantityText.text = string.Empty;
      }

      //2. 点击Slot中的物品时进行选中
      public void OnClickButton()
      {
         Inventory.instance.SelectItem(index);

         useButton.SetActive(currentslot.item.type == ItemType.Consumable);
         equipButton.SetActive(currentslot.item.type == ItemType.Wieldable && !equipped);
         dropItemButton.SetActive(currentslot.item.type == ItemType.Wieldable && equipped);
         dropButton.SetActive(true);
      }

      
   }


}
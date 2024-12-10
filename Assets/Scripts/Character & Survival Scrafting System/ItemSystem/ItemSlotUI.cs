using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LPSurvivalEngine
{
   public class ItemSlotUI : MonoBehaviour
   {
   [Space]
   [Header("Item Slot")]
   [Space]
   [Space]

   [Space]
   [Header("Assignments")]
   [Space]

   public Button button;
   public Image icon;
   public TextMeshProUGUI quantityText;
   private ItemSlot currentslot;

   [Space]
   [Header("Settings")]
   [Space]

   public int index;
   public bool equipped;
   

   public void Set(ItemSlot slot)
   {
      currentslot = slot;
      
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

   public void OnClickButton()
   {
      Inventory.instance.SelectItem(index);
   }
   
}


}
using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    // [Header("Description")]
    // public TextMeshProUGUI selectedItemName;
    // public TextMeshProUGUI selectedItemDescription;

    public Inventory playerInventory;  
    public ContainerInventory containerInventory; 

    public ItemSlotUI[] inventorySlotUIs;  
    public ItemSlotUI[] containerSlotUIs;  


    public void OpenContainerUI()
    {
        for (int i = 0; i < playerInventory.slots.Length; i++)
        {
            inventorySlotUIs[i].Set(playerInventory.slots[i]); 
        }

        for (int i = 0; i < containerInventory.containerSlots.Length; i++)
        {
            containerSlotUIs[i].Set(containerInventory.containerSlots[i]); 
        }
    }


    // public void OnItemDraggedToInventory(int index)
    // {
    //     playerInventory.TransferItemToInventory(index);
    //     OpenContainerUI();
    // }

    // public void OnItemDraggedToContainer(int index)
    // {
    //     playerInventory.TransferItemToContainer(index);
    //     OpenContainerUI();
    // }
}

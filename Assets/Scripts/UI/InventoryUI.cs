using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;   // 玩家背包
    public ContainerInventory containerInventory; // 箱子

    public ItemSlotUI[] inventorySlotUIs;  // 显示玩家背包的UI
    public ItemSlotUI[] containerSlotUIs;  // 显示箱子的UI

    // 切换到箱子的物品栏时更新UI
    public void OpenContainerUI()
    {
        for (int i = 0; i < playerInventory.slots.Length; i++)
        {
            inventorySlotUIs[i].Set(playerInventory.slots[i]); // 更新背包UI
        }

        for (int i = 0; i < containerInventory.containerSlots.Length; i++)
        {
            containerSlotUIs[i].Set(containerInventory.containerSlots[i]); // 更新箱子UI
        }
    }

    // 拖动物品到背包
    public void OnItemDraggedToInventory(int index)
    {
        playerInventory.TransferItemToInventory(index);
        OpenContainerUI();
    }

    // 拖动物品到箱子
    public void OnItemDraggedToContainer(int index)
    {
        playerInventory.TransferItemToContainer(index);
        OpenContainerUI();
    }
}

using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ContainerInventory : MonoBehaviour
    {
        public ItemSlot[] containerSlots; // 箱子中的物品槽

        private void Start()
        {
            containerSlots = new ItemSlot[10]; // 例如，箱子最多有10个槽位
            for (int i = 0; i < containerSlots.Length; i++)
            {
                containerSlots[i] = new ItemSlot();
            }
        }

        // 添加物品到箱子
        public bool AddItemToContainer(ItemDatabase item)
        {
            for (int i = 0; i < containerSlots.Length; i++)
            {
                if (containerSlots[i].item == null)
                {
                    containerSlots[i].item = item;
                    containerSlots[i].quantity = 1;
                    return true;
                }
            }
            return false; // 箱子已满
        }

        // 从箱子中移除物品
        public void RemoveItemFromContainer(int index)
        {
            if (containerSlots[index].item != null)
            {
                containerSlots[index].item = null;
                containerSlots[index].quantity = 0;
            }
        }
    }
}


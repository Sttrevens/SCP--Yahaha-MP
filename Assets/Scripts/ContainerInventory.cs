using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ContainerInventory : MonoBehaviour
    {
        public ItemSlot[] containerSlots; // �����е���Ʒ��


        private void Start()
        {
            containerSlots = new ItemSlot[9]; // ���磬���������10����λ
            for (int i = 0; i < containerSlots.Length; i++)
            {
                containerSlots[i] = new ItemSlot();
                
            }
        }

        public void AddItemToContainer(ItemDatabase item)
        {
            if (item.canStackItem)
            {
                ItemSlot slotToStackTo = GetItemstack(item);

                if (slotToStackTo != null)
                {
                    slotToStackTo.quantity++;
                    //Debug.Log(slotToStackTo.item.name + slotToStackTo.quantity);
                    return;
                }
            }

            ItemSlot emptySlot = GetEmptySlot();
            if (emptySlot != null) //如果有空位
            {
                emptySlot.item = item;
                emptySlot.quantity = 1;
                return;
            }
        }

        // public bool AddItemToContainer(ItemDatabase item)
        // {
        //     for (int i = 0; i < containerSlots.Length; i++)
        //     {
        //         if (containerSlots[i].item == null)
        //         {
        //             containerSlots[i].item = item;
        //             containerSlots[i].quantity = 1;
        //             return true;
        //         }
        //     }
        //     return false; // ��������
        // }

         ItemSlot GetItemstack(ItemDatabase item) 
        {
            for (int x = 0; x < containerSlots.Length; x++)
            {
                if (containerSlots[x].item == item && containerSlots[x].quantity < item.maxStackamount)
                {
                    return containerSlots[x];
                }
            }
            return null;
        }


        ItemSlot GetEmptySlot()
        {
            for (int x = 0; x < containerSlots.Length; x++)
            {
                if (containerSlots[x].item == null)
                {
                    return containerSlots[x];
                }
            }
            return null;
        }


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


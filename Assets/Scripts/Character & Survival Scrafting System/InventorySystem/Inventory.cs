using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using System;

namespace LPSurvivalEngine
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory instance{get;private set;}

        public GameObject bagPanel; //包含物品槽的背包父物体
        private ItemSlotUI[] InventorySlots;
        public ItemSlot[] slots;


        [Header("Assignments")]
        public GameObject inventoryWindow;
        public GameObject containerUIWindow;
        public Transform dropPosition;

        // [Space]
        // [Header("UI")]
        // [Space]
        // [Header("Texts")]
        // public TextMeshProUGUI selectedItemName;
        // public TextMeshProUGUI selectedItemDescription;

        // [Space]
        // [Header("Buttons")]
        // public GameObject useButton;
        // public GameObject equipButton;
        // public GameObject dropItemButton;
        // public GameObject dropButton;

        [Space]
        [Header("Events")]
        public UnityEvent onOpenInventory;
        public UnityEvent onCloseInventory;
        public UnityEvent onCloseContainerInventory;

        private int selectedItemIndex;
        private PlayerController playerController;
        private HealthSystem vitals;
        private ItemSlot selectedItem;


        public ContainerInventory containerInventory;
        private int currentWieldableIndex;
        public ItemSlotUI[] containerSlots;// UI���ڣ���ʾ���Ӻͱ�������Ʒ

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject); 
            }
        }

        private void Start()
        {
            inventoryWindow.SetActive(false);
            InventorySlots = bagPanel.GetComponentsInChildren<ItemSlotUI>();

            slots = new ItemSlot[InventorySlots.Length];

            for (int x = 0; x < slots.Length; x++)
            {
                slots[x] = new ItemSlot();
                InventorySlots[x].index = x;
                InventorySlots[x].Clear();
            }
            //ClearSelectedItemWindow();

            playerController = PlayerController.instance;
            vitals = HealthSystem.instance;
        }

        public void OnInventoryButton(InputAction.CallbackContext context)
        {
            Debug.Log("OnInventoryButton triggered by: " + context.action.name);
            if (context.phase == InputActionPhase.Started)
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (inventoryWindow.activeInHierarchy)
            {
                inventoryWindow.SetActive(false);
                onCloseInventory.Invoke();
                playerController.ToggleCursor(false);
            }
            else if (containerUIWindow.activeInHierarchy)
            {
                containerUIWindow.SetActive(false);
                onCloseContainerInventory.Invoke();
                playerController.ToggleCursor(false);
            }
            else
            {
                inventoryWindow.SetActive(true);
                onOpenInventory.Invoke();
                //ClearSelectedItemWindow();
                playerController.ToggleCursor(true);
            }
        }

        public bool isOpen()
        {
            return inventoryWindow.activeInHierarchy;
        }

        // ������Ʒ������
        public void AddItem(ItemDatabase item)
        {
            if (item.canStackItem)
            {
                ItemSlot slotToStackTo = GetItemstack(item);

                if (slotToStackTo != null)
                {
                    slotToStackTo.quantity++;
                    UpdateUI();
                    return;
                }
            }

            ItemSlot emptySlot = GetEmptySlot();
            if (emptySlot != null) //如果有空位
            {
                emptySlot.item = item;
                emptySlot.quantity = 1;
                UpdateUI();
                return;
            }

            //如果物品槽已满
            ThrowItem(item);
            Prompt.instance.CustomPrompt("Your Bag is already full!"); //显示提示
        }

        void ThrowItem(ItemDatabase item)
        {
            Prompt.instance.CustomPrompt(String.Format("{0} has been thrown!", selectedItem.item.name)); //显示提示
            Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360.0f));
        }

        void UpdateUI()
        {
            for (int x = 0; x < slots.Length; x++)
            {
                if (slots[x].item != null)
                {
                    InventorySlots[x].Set(slots[x]);
                }
                else
                {
                    InventorySlots[x].Clear();
                }
            }
        }


        ItemSlot GetItemstack(ItemDatabase item) 
        {
            for (int x = 0; x < slots.Length; x++)
            {
                if (slots[x].item == item && slots[x].quantity < item.maxStackamount)
                {
                    return slots[x];
                }
            }
            return null;
        }


        ItemSlot GetEmptySlot()
        {
            for (int x = 0; x < slots.Length; x++)
            {
                if (slots[x].item == null)
                {
                    return slots[x];
                }
            }
            return null;
        }

       
        public void SelectItem(int index)
        {
            if (slots[index].item == null)
                return;

            selectedItem = slots[index];
            selectedItemIndex = index;

            if (selectedItem.item.type == ItemType.Consumable) UseConsumableItem();
            else if (selectedItem.item.type == ItemType.Wieldable) EquipWieldableItem();
            else Prompt.instance.SlotItemPrompt(selectedItem.item); //显示提示
        }

        void UseConsumableItem()
        {
            Prompt.instance.SlotItemPrompt(selectedItem.item); //显示提示     
            for (int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch (selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: vitals.Heal(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Hunger: vitals.Eat(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Thirst: vitals.Drink(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Sleep: vitals.Sleep(selectedItem.item.consumables[x].value); break;
                }
            }
            RemoveSelectedItem();
            
        }

        // public void OnDisableButton()
        // {
        //     DisableItem(selectedItemIndex);
        // }

        public void OnDropButton()
        {
            if (selectedItem.item == null)
                return;

            ThrowItem(selectedItem.item);
            RemoveSelectedItem();
        }

        public void EquipWieldableItem()
        {
            if (InventorySlots[currentWieldableIndex].equipped)
                {
                    DisableItem(currentWieldableIndex);
                    if (currentWieldableIndex == selectedItemIndex)  
                    {
                        Prompt.instance.CustomPrompt(String.Format("{0} unequipped!", selectedItem.item.name)); //显示提示
                        return; 
                    }
                }

            Prompt.instance.SlotItemPrompt(selectedItem.item); //显示提示

            InventorySlots[selectedItemIndex].equipped = true;
            currentWieldableIndex = selectedItemIndex;
            WieldableManager.instance.EquipNewItem(selectedItem.item);
            UpdateUI();
            // SelectItem(selectedItemIndex);
        }

        void DisableItem(int index)
        {
            InventorySlots[index].equipped = false;

            WieldableManager.instance.DropWieldable();

            UpdateUI();

            // if (selectedItemIndex == index)
            //     SelectItem(index);
        }

        void RemoveSelectedItem()
        {
            selectedItem.quantity--;

            if (selectedItem.quantity == 0)
            {
                if (InventorySlots[selectedItemIndex].equipped == true)
                    DisableItem(selectedItemIndex);
                selectedItem.item = null;
                //ClearSelectedItemWindow();
            }
            UpdateUI();
        }

        public void RemoveItem(ItemDatabase item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item)
                {
                    slots[i].quantity--;

                    if (slots[i].quantity == 0)
                    {
                        if (InventorySlots[i].equipped == true)
                            DisableItem(i);
                        slots[i].item = null;
                        //ClearSelectedItemWindow();
                    }
                    UpdateUI();
                    return;
                }
            }
        }

        public void InteractWithContainer()
        {
            containerUIWindow.SetActive(true);  // ��ʾ���ӵ�UI����
            UpdateContainerUI();  // ����UI����ʾ���Ӻͱ����е���Ʒ
            playerController.ToggleCursor(true);
        }

        // �������Ӻͱ�����UI
        public void UpdateContainerUI()
        {
            // ���±�������Ʒ��
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    InventorySlots[i].Set(slots[i]);
                }
                else
                {
                    InventorySlots[i].Clear();
                }
            }

            // �������ӵ���Ʒ��
            for (int i = 0; i < containerInventory.containerSlots.Length; i++)
            {
                if (containerInventory.containerSlots[i].item != null)
                {
                    // ��������һ�������ڱ�����λUI�������������ʾ�����е���Ʒ
                    containerSlots[i].Set(containerInventory.containerSlots[i]);
                }
                /*else
                {
                    containerSlots[i].Clear();
                }*/
            }
        }

        // ����Ʒ�ӱ���ת�Ƶ�����
        public void TransferItemToContainer(int inventoryIndex)
        {
            ItemSlot inventorySlot = slots[inventoryIndex];
            if (inventorySlot.item != null)
            {
                for (int i = 0; i < containerInventory.containerSlots.Length; i++)
                {
                    if (containerInventory.containerSlots[i].item == null)
                    {
                        containerInventory.containerSlots[i].item = inventorySlot.item;
                        containerInventory.containerSlots[i].quantity = inventorySlot.quantity;
                        inventorySlot.item = null;
                        inventorySlot.quantity = 0;
                        UpdateContainerUI();
                        return;
                    }
                }
            }
        }

        // ����Ʒ������ת�Ƶ�����
        public void TransferItemToInventory(int containerIndex)
        {
            ItemSlot containerSlot = containerInventory.containerSlots[containerIndex];
            if (containerSlot.item != null)
            {
                ItemSlot emptySlot = GetEmptySlot();
                if (emptySlot != null)
                {
                    emptySlot.item = containerSlot.item;
                    emptySlot.quantity = containerSlot.quantity;
                    containerSlot.item = null;
                    containerSlot.quantity = 0;
                    UpdateContainerUI();
                }
            }
        }

        public bool HasItems(ItemDatabase item, int quantity)
        {
            int amount = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item)
                    amount += slots[i].quantity;

                if (amount >= quantity)
                    return true;
            }
            return false;
        }
    }
}


namespace LPSurvivalEngine
{
    public class ItemSlot
    {
        public ItemDatabase item;
        public int quantity;
    }
}
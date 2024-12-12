using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace LPSurvivalEngine
{
    public class Inventory : MonoBehaviour
    {
        [Space]
        [Header("Inventory System")]
        [Space]

        public ItemSlotUI[] IventorySlots;
        public ItemSlot[] slots;

        [Space]
        [Header("Assignments")]
        [Space]

        public GameObject inventoryWindow;
        public Transform dropPosition;

        [Space]
        [Header("UI")]
        [Space]
        [Header("Texts")]
        public TextMeshProUGUI selectedItemName;
        public TextMeshProUGUI selectedItemDescription;

        [Space]
        [Header("Buttons")]
        public GameObject useButton;
        public GameObject equipButton;
        public GameObject dropItemButton;
        public GameObject dropButton;

        [Space]
        [Header("Events")]
        public UnityEvent onOpenInventory;
        public UnityEvent onCloseInventory;
        public UnityEvent onCloseContainerInventory;

        private int selectedItemIndex;
        private PlayerController playerController;
        private HealthSystem vitals;
        private ItemSlot selectedItem;
        public static Inventory instance;

        // �������������ӵ���Ʒ��
        public ContainerInventory containerInventory;
        private int currentWieldableIndex;
        public GameObject containerUIWindow;
        public ItemSlotUI[] containerSlots;// UI���ڣ���ʾ���Ӻͱ�������Ʒ

        private void Awake()
        {
            instance = this;
            playerController = GetComponent<PlayerController>();
            vitals = GetComponent<HealthSystem>();
        }

        private void Start()
        {
            inventoryWindow.SetActive(false);
            slots = new ItemSlot[IventorySlots.Length];

            for (int x = 0; x < slots.Length; x++)
            {
                slots[x] = new ItemSlot();
                IventorySlots[x].index = x;
                IventorySlots[x].Clear();
            }
            ClearSelectedItemWindow();
        }

        public void OnInventoryButton(InputAction.CallbackContext context)
        {
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
                ClearSelectedItemWindow();
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
            if (emptySlot != null)
            {
                emptySlot.item = item;
                emptySlot.quantity = 1;
                UpdateUI();
                return;
            }
            ThrowItem(item);
        }

        // ����Ʒ����
        void ThrowItem(ItemDatabase item)
        {
            Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
        }

        void UpdateUI()
        {
            for (int x = 0; x < slots.Length; x++)
            {
                if (slots[x].item != null)
                {
                    IventorySlots[x].Set(slots[x]);
                }
                else
                {
                    IventorySlots[x].Clear();
                }
            }
        }

        // ����Ʒ��ջ�л�ȡ�ղ�
        ItemSlot GetItemstack(ItemDatabase item) // TODO: 能否改成map，提高查找效率
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

        // ��ȡ�յ���Ʒ��
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

        // ѡ����Ʒ
        public void SelectItem(int index)
        {
            if (slots[index].item == null)
                return;

            selectedItem = slots[index];
            selectedItemIndex = index;

            selectedItemName.text = selectedItem.item.displayName;
            selectedItemDescription.text = selectedItem.item.description;

            useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
            equipButton.SetActive(selectedItem.item.type == ItemType.Wieldable && !IventorySlots[index].equipped);
            dropItemButton.SetActive(selectedItem.item.type == ItemType.Wieldable && IventorySlots[index].equipped);
            dropButton.SetActive(true);
        }

        void ClearSelectedItemWindow()
        {
            selectedItem = null;
            selectedItemName.text = string.Empty;
            selectedItemDescription.text = string.Empty;

            useButton.SetActive(false);
            equipButton.SetActive(false);
            dropItemButton.SetActive(false);
            dropButton.SetActive(false);
        }

        public void OnUseButton()
        {
            if (selectedItem.item.type == ItemType.Consumable)
            {
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
            }
            RemoveSelectedItem();
        }

        void DisableItem(int index)
        {
            IventorySlots[index].equipped = false;

            WieldableManager.instance.DropWieldable();

            UpdateUI();

            if (selectedItemIndex == index)
                SelectItem(index);
        }

        public void OnDisableButton()
        {
            DisableItem(selectedItemIndex);
        }

        public void OnDropButton()
        {
            ThrowItem(selectedItem.item);
            RemoveSelectedItem();
        }

        public void OnUseItemButton()
        {
            if (IventorySlots[currentWieldableIndex].equipped)
                DisableItem(currentWieldableIndex);

            IventorySlots[selectedItemIndex].equipped = true;
            currentWieldableIndex = selectedItemIndex;
            WieldableManager.instance.EquipNewItem(selectedItem.item);
            UpdateUI();
            SelectItem(selectedItemIndex);
        }

        void RemoveSelectedItem()
        {
            selectedItem.quantity--;

            if (selectedItem.quantity == 0)
            {
                if (IventorySlots[selectedItemIndex].equipped == true)
                    DisableItem(selectedItemIndex);
                selectedItem.item = null;
                ClearSelectedItemWindow();
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
                        if (IventorySlots[i].equipped == true)
                            DisableItem(i);
                        slots[i].item = null;
                        ClearSelectedItemWindow();
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
                    IventorySlots[i].Set(slots[i]);
                }
                else
                {
                    IventorySlots[i].Clear();
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
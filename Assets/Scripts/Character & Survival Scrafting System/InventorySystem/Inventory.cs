using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using Fusion;
using UnityEngine.SceneManagement;
using System.Globalization;

namespace LPSurvivalEngine
{
    public class Inventory : NetworkBehaviour
    {
        public static Inventory instance{get;private set;}


        [Header("Assignments")]
        public GameObject inventoryWindow; //菜单界面(InventoryCanvas)
        public GameObject bagPanel; //背包面板（QuickSlot)
        [HideInInspector] public ItemSlot[] slots;
        private ItemSlotUI[] InventorySlots;

        public GameObject containerUIWindow; //容器界面（ContainerCanvas)
        public GameObject containerPanel; //容器面板（ContainerPanel)
        private ItemSlotUI[] containerSlots;
        private ContainerInventory currentContainerInventory;

        public Transform dropPosition;


        [Header("Events")]
        public UnityEvent onOpenInventory;
        public UnityEvent onCloseInventory;
        public UnityEvent onCloseContainerInventory;
        
        [Header("Input")]
        public PlayerInput PlayerInput;
        private InputAction inventoryAction;

        public int selectedItemIndex;
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public HealthSystem vitals;
        [HideInInspector]public ItemSlot selectedItem;
        private int currentWieldableIndex;


        private void Awake()
        {
            //PlayerInput = GameObject.Find("InputManager").GetComponent<PlayerInput>();
            
            /*if (PlayerInput != null) {
                inventoryAction = PlayerInput.actions.FindAction("Inventory");
            }*/
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

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //inventoryAction.started += OnInventoryButton;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            //inventoryAction.started -= OnInventoryButton;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            selectedItem = null;
            Debug.Log("Scene loaded and static variables reset.");
        }

        private void Start()
        {
            inventoryWindow.SetActive(false);
            InventorySlots = bagPanel.GetComponentsInChildren<ItemSlotUI>();
            containerSlots = containerPanel.GetComponentsInChildren<ItemSlotUI>();

            slots = new ItemSlot[InventorySlots.Length];

            for (int x = 0; x < slots.Length; x++)
            {
                slots[x] = new ItemSlot();
                InventorySlots[x].index = x;
                InventorySlots[x].Clear();
            }
            //ClearSelectedItemWindow();

            playerController = PlayerController.instance;

            if (InventorySlots == null || InventorySlots.Length == 0)
            {
                Debug.LogError("InventorySlots has not been properly initialized.");
            }

            if (containerSlots == null || containerSlots.Length == 0)
            {
                Debug.LogError("ContainerSlots has not been properly initialized.");
            }
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
            //TODO:关闭Bag面板的交互
            bagPanel.GetComponent<CanvasGroup>().interactable = false;
            if (inventoryWindow.activeInHierarchy)
            {
                inventoryWindow.SetActive(false);
                onCloseInventory.Invoke();
                //playerController.ToggleCursor(false);
            }
            else if (containerUIWindow.activeInHierarchy)
            {
                containerUIWindow.SetActive(false);
                onCloseContainerInventory.Invoke();
                //playerController.ToggleCursor(false);
            }
            else
            {
                inventoryWindow.SetActive(true);
                onOpenInventory.Invoke();
                //ClearSelectedItemWindow();
                //playerController.ToggleCursor(true);
            }
        }

        public bool isOpen()
        {
            return inventoryWindow.activeInHierarchy;
        }

        // 新增：专门处理拾取物品的方法
public void PickupItem(ItemObject itemObject)
{
    if (itemObject != null)
    {
        AddItem(itemObject.item, itemObject.currentDurability); // 确保传递当前耐久度
    }
}
        
        public void AddItem(ItemDatabase item, float durability = -1)
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
                emptySlot.currentDurability = durability >= 0 ? durability : item.maxDurability;
                UpdateUI();
                return;
            }

            ThrowItem(item);
            Prompt.instance.CustomPrompt("Your Bag is already full!");
        }

        public void ThrowItem(ItemDatabase item)
        {
            Prompt.instance.CustomPrompt(string.Format("{0} has been thrown!", selectedItem.item.name));
            throwedItem = item;
            currentThrowingItemDurability = selectedItem.currentDurability;
            RequestStateAuthorityForEquipItem(Runner.LocalPlayer);
            RPC_RequestSpawnItem(Runner.LocalPlayer);

            if (item.type == ItemType.Wieldable)
            {
                WieldableManager.instance.DropWieldable();
            }
        }

        private ItemDatabase throwedItem;

        [Networked] public PlayerRef Owner { get; set; } // 网络同步的物品所有者

        // RPC 请求生成物品（客户端调用）
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestSpawnItem(PlayerRef player)
        {
            // 只有 StateAuthority 才能执行 Runner.Spawn
            if (Object.HasStateAuthority)
            {
                dropPosition = GameObject.Find("PublicDropBox").transform;
                SpawnItem(player);
            }
        }

        private void RequestStateAuthorityForEquipItem(PlayerRef player)
        {
            // 如果当前客户端没有 StateAuthority，尝试请求
            if (!HasStateAuthority)
            {
                // 此代码段表示此对象在当前客户端上没有控制权限
                Debug.Log("Requesting StateAuthority for EquipItem.");
                Object.RequestStateAuthority();
                if (HasStateAuthority)
                {
                    Debug.Log($"This client has StateAuthority over {gameObject.name}");
                }
                else
                {
                    Debug.Log($"This client does not have StateAuthority over {gameObject.name}");
                }
            }// 请求获取该对象的控制权限
            else
            {
                Debug.Log("Already have StateAuthority.");
            }
        }

        // 物品生成逻辑（只在 StateAuthority 执行）
        private void SpawnItem(PlayerRef player)
        {
            Owner = player;

            // 物品实例化的旋转可以根据需要调整
            Quaternion randomRotation = Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360.0f);

            // 使用 Runner.Spawn 实例化并同步物品
            NetworkObject spawnedItem = Runner.Spawn(throwedItem.dropPrefab, dropPosition.position, randomRotation);

            // 确保新生成的物品有正确的所有者
            if (spawnedItem.TryGetComponent<ItemObject>(out var itemObject))
            {
                itemObject.Owner = player;
                itemObject.IsPickedUp = false;
                itemObject.currentDurability = currentThrowingItemDurability;
            }
        }


        public void UpdateUI()
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
            //Debug.Log("Current Selected Item: " + selectedItem);
            if (slots[index].item == null)
                return;

            selectedItem = slots[index];
            selectedItemIndex = index;

            if (selectedItem.item != null)
            {
                if (selectedItem.item.type == ItemType.Consumable)
                    Prompt.instance.SlotItemPrompt(selectedItem.item);
                else if (selectedItem.item.type == ItemType.Wieldable)
                    EquipWieldableItem();
                else
                    Prompt.instance.SlotItemPrompt(selectedItem.item); //显示提示
            }
        }

        void UseConsumableItem()
        {
            Debug.Log("调用Consumable喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵");
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
            Prompt.instance.UseItemPrompt(selectedItem.item);

        }

        // public void OnDisableButton()
        // {
        //     DisableItem(selectedItemIndex);
        // }

        public void DropItem()
        {
            if (selectedItem== null || selectedItem.item == null)
                return;

            ThrowItem(selectedItem.item);
            RemoveSelectedItem();
        }

        public void UseItem()
        {
            if (selectedItem== null || selectedItem.item == null)
                return;

            if (selectedItem.item.type == ItemType.Consumable)
                UseConsumableItem();
            
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

        public void RemoveSelectedItem()
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

        public void InteractWithContainer(ContainerInventory containerInventory)
        {
            currentContainerInventory = containerInventory;
            containerUIWindow.SetActive(true);  
            UpdateContainerUI();  
            playerController.ToggleCursor(true);
            //TODO:打开Bag面板的交互
            bagPanel.GetComponent<CanvasGroup>().interactable = true;
        }

        public void UpdateContainerUI()
        {
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
            
            for (int i = 0; i < currentContainerInventory.containerSlots.Length; i++)
            {
                if (currentContainerInventory.containerSlots[i].item != null)
                {
                    containerSlots[i].Set(currentContainerInventory.containerSlots[i]);
                }
                else
                {
                    containerSlots[i].Clear();
                }
            }
        }
        
        public void TransferItemToContainer(int inventoryIndex)
        {
            ItemSlot inventorySlot = slots[inventoryIndex];
            if (inventorySlot.item != null)
            {
                currentContainerInventory.AddItemToContainer(inventorySlot.item);
                inventorySlot.quantity--;
                if(inventorySlot.quantity==0)
                {
                    inventorySlot.item = null;
                }
                UpdateContainerUI();
            }
        }
        
        public void TransferItemToInventory(int containerIndex)
        {
            ItemSlot containerSlot = currentContainerInventory.containerSlots[containerIndex];
            if (containerSlot.item != null)
            {
                AddItem(containerSlot.item);
                containerSlot.quantity--;
                if(containerSlot.quantity==0)
                {
                    containerSlot.item = null;
                }
                
                UpdateContainerUI();
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

        [Networked] private float currentThrowingItemDurability { get; set; }

        public void UpdateItemDurability(int slotIndex, float durabilityChange)
        {
            if (slots[slotIndex].item != null && slots[slotIndex].item.type == ItemType.Wieldable)
            {
                slots[slotIndex].currentDurability = Mathf.Max(0, slots[slotIndex].currentDurability - durabilityChange);
                
                // 如果耐久度降为0，可以选择销毁物品
                if (slots[slotIndex].currentDurability <= 0)
                {
                    if (InventorySlots[slotIndex].equipped)
                    {
                        DisableItem(slotIndex);
                    }
                    slots[slotIndex].item = null;
                    slots[slotIndex].quantity = 0;
                }
                
                UpdateUI();
            }
        }
    }
}


namespace LPSurvivalEngine
{
    public class ItemSlot
    {
        public ItemDatabase item;
        public int quantity;
        public float currentDurability;
    }
}
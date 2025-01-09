using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

namespace LPSurvivalEngine
{
    public class WieldableManager : NetworkBehaviour
    {  
        [Space]
        [Header("Wieldable Manager")]
        [Space]
        [Space]
    
        [HideInInspector] public Wieldable currentWieldable;
        public Transform wieldablesPosition;
        public Transform flashlightPosition;
        public Transform cameraPositon;
        public Transform AimPositon;
        public PlayerInput PlayerInput;
        private InputAction actionAction;
    
        public static WieldableManager instance;
        [HideInInspector] public PlayerController controller;


        private void Awake()
        {
            instance = this;
            // PlayerInput = GameObject.Find("InputManager").GetComponent<PlayerInput>();
            
            //if (PlayerInput != null) {
            //    actionAction = PlayerInput.actions.FindAction("Action");
            //    actionAction.performed += OnAttackInput;
            //}
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            Debug.Log("WieldAble喵");
            if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
            {
                currentWieldable.OnAttackInput();
            }
        }
    
        public void OnAltAttackInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
            {
                currentWieldable.OnAltAttackInput();
            }
        }

        // 联机版 EquipNewItem 方法
        public void EquipNewItem(ItemDatabase item)
        {
            // 记录当前要装备的物品
            equippedItem = item;

            // 请求服务器生成物品
            RPC_RequestEquipItem(Runner.LocalPlayer);
        }

        // 记录当前要装备的物品
        private ItemDatabase equippedItem;

        [Networked] public PlayerRef Owner { get; set; } // 网络同步的物品所有者

        // RPC 请求生成装备物品（客户端调用）
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            // 只有 StateAuthority 才能执行 Spawn
            if (Object.HasStateAuthority)
            {
                SpawnEquippedItem(player);
            }
        }

        // 物品生成逻辑（只在 StateAuthority 执行）
        private void SpawnEquippedItem(PlayerRef player)
        {
            Owner = player;

            // 根据物品类型选择生成位置
            Transform spawnPosition = null;
            if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<CameraController>() == null)
            {
                GameObject currentPlayer = GameObject.Find("CurrentPlayer");
                spawnPosition = currentPlayer.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<CameraController>() != null)
            {
                spawnPosition = cameraPositon;
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() != null && equippedItem.wieldablePrefab.GetComponent<CameraController>() == null)
            {
                spawnPosition = flashlightPosition;
            }

            // 如果没有找到生成位置，抛出错误
            if (spawnPosition == null)
            {
                Debug.LogError("Unexpected item type: " + equippedItem.wieldablePrefab.name);
                return;
            }

            // 使用 Runner.Spawn 实例化并同步物品
            NetworkObject spawnedItem = Runner.Spawn(equippedItem.wieldablePrefab, spawnPosition.position, spawnPosition.rotation);

            // 确保新生成的物品挂载到父物体
            if (spawnedItem != null)
            {
                // 设置物品的父物体
                spawnedItem.transform.SetParent(spawnPosition);

                // 重置生成物品的本地位置和旋转
                spawnedItem.transform.localPosition = Vector3.zero;
                spawnedItem.transform.localRotation = Quaternion.identity;

                // 设置为当前装备物品
                if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
                {
                    currentWieldable = wieldable; // 更新当前装备的物品
                    Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");
                }
            }
            else
            {
                Debug.LogError("Failed to spawn item: " + equippedItem.wieldablePrefab.name);
            }
        }


        public void DropWieldable()
        {
            if (currentWieldable != null)
            {
                Destroy(currentWieldable.gameObject);
                currentWieldable = null;
            }
        }
    
    
    }


}
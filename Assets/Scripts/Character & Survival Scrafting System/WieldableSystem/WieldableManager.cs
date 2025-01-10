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
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            // 调用生成物品的逻辑
            SpawnEquippedItem(player);
        }

        public Transform CurrentWieldableRootTransform()
        {
            Transform spawnPosition = null;
            if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() == null)
            {
                GameObject currentPlayer = GameObject.Find("CurrentPlayer");
                spawnPosition = currentPlayer.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() != null)
            {
                spawnPosition = cameraPositon;
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() != null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() == null)
            {
                spawnPosition = flashlightPosition;
            }

            return spawnPosition;
        }

        // 物品生成逻辑（生成时在正确的玩家控制下执行）
        private void SpawnEquippedItem(PlayerRef player)
        {
            Owner = player;

            // 确保父物体设置有效
            Transform spawnPosition = CurrentWieldableRootTransform();
            if (spawnPosition == null)
            {
                Debug.LogError("Unexpected item type: " + equippedItem.wieldablePrefab.name);
                return;
            }

            // 使用 Runner.Spawn 实例化并同步物品
            NetworkObject spawnedItem = Runner.Spawn(equippedItem.wieldablePrefab, spawnPosition.position, spawnPosition.rotation);

            if (spawnedItem != null)
            {
                spawnedItem.transform.position = spawnPosition.position;
                spawnedItem.transform.rotation = spawnPosition.rotation;

                // 设置物品的父物体
                spawnedItem.transform.SetParent(spawnPosition);

                // 重置生成物品的本地位置和旋转
                if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
                {
                    currentWieldable = wieldable; // 更新当前装备的物品
                    Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");

                    // 通过 StateAuthority 设置物品的所有者
                    // 只有拥有 StateAuthority 的客户端才可以进行此操作
                    if (spawnedItem.HasStateAuthority)
                    {
                        // 只有获得 StateAuthority 的客户端可以控制物品
                        spawnedItem.RequestStateAuthority();  // 确保当前客户端拥有物品控制权
                    }

                    currentWieldable.player = player;
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

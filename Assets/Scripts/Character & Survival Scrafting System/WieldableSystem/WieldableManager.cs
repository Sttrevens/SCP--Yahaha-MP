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

        public Transform wieldablesPosition;
        public Transform flashlightPosition;
        public Transform cameraPositon;
        public Transform aimPositon;
        [SerializeField] private PlayerInput playerInput;

        [HideInInspector] public Wieldable currentWieldable;
        [HideInInspector] public PlayerController controller;
        [Networked] public PlayerRef Owner { get; set; }

        private ItemDatabase equippedItem;
        public static WieldableManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (!IsValidWieldableAction(context)) return;
            
            currentWieldable.OnAttackInput();
        }

        public void OnAltAttackInput(InputAction.CallbackContext context) 
        {
            if (!IsValidWieldableAction(context)) return;

            currentWieldable.OnAltAttackInput();
        }

        private bool IsValidWieldableAction(InputAction.CallbackContext context)
        {
            return context.phase == InputActionPhase.Performed && 
                   currentWieldable != null && 
                   controller.cursor;
        }

        private int currentWieldableIndex = -1;

        public ItemSlot GetCurrentWieldableSlot()
        {
            if (currentWieldableIndex >= 0)
            {
                return Inventory.instance.slots[currentWieldableIndex];
            }
            return null;
        }

        public int GetCurrentWieldableIndex()
        {
            return currentWieldableIndex;
        }

        public void EquipNewItem(ItemDatabase item)
        {
            Debug.Log($"EquipNewItem called by player {Runner.LocalPlayer}");
            equippedItem = item;
            currentWieldableIndex = Inventory.instance.selectedItemIndex;
            
            // 直接调用RPC，不需要请求StateAuthority
            RPC_RequestEquipItem(Runner.LocalPlayer);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;
            Debug.Log($"RPC_RequestEquipItem received for player {player}");
            SpawnEquippedItem(player);
        }

        private void SpawnEquippedItem(PlayerRef player)
        {
            Owner = player;
            Debug.Log($"SpawnEquippedItem for player {player}");

            Transform spawnTransform = CurrentWieldableRootTransform();
            if (spawnTransform == null)
            {
                Debug.LogError($"Unexpected item type or invalid transform: {equippedItem?.wieldablePrefab?.name}");
                return;
            }

            NetworkObject spawnedItem = Runner.Spawn(
                equippedItem.wieldablePrefab, 
                spawnTransform.position, 
                spawnTransform.rotation,
                player  // 指定所有者
            );

            if (spawnedItem == null)
            {
                Debug.LogError($"Failed to spawn item: {equippedItem.wieldablePrefab.name}");
                return;
            }

            SetupSpawnedItem(spawnedItem, spawnTransform, player);
        }

        private void SetupSpawnedItem(NetworkObject spawnedItem, Transform parent, PlayerRef player)
        {
            Debug.Log($"SetupSpawnedItem for player {player}");
            
            // 在所有客户端上执行
            spawnedItem.transform.SetParent(parent);
            spawnedItem.transform.localPosition = Vector3.zero;
            spawnedItem.transform.localRotation = Quaternion.identity;
            spawnedItem.transform.localScale = Vector3.one;

            if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
            {
                currentWieldable = wieldable;
                currentWieldable.player = player;
                Debug.Log($"Wieldable setup complete for player {player}");
            }
        }

        public Transform CurrentWieldableRootTransform()
        {
            if (equippedItem == null || equippedItem.wieldablePrefab == null) return null;

            var prefab = equippedItem.wieldablePrefab;
            bool hasFlashlight = prefab.GetComponent<Flashlight>() != null;
            bool hasConeDetection = prefab.GetComponent<ConeDetection>() != null;

            // 获取正确的玩家对象
            NetworkObject playerObject = Runner.GetPlayerObject(Owner);
            if (playerObject == null)
            {
                Debug.LogError($"Could not find player object for Owner {Owner}");
                return null;
            }

            Transform targetTransform = null;
            string path = "";

            if (!hasFlashlight && !hasConeDetection)
            {
                path = "Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR";
            }
            else if (!hasFlashlight && hasConeDetection)
            {
                path = "UpperBody/CameraRoot/HoldCameraRoot";
            }
            else if (hasFlashlight && !hasConeDetection)
            {
                path = "UpperBody/CameraRoot/FlashlightRoot";
            }

            targetTransform = playerObject.transform.Find(path);
            if (targetTransform == null)
            {
                Debug.LogError($"Could not find transform at path: {path} for player {Owner}");
            }

            return targetTransform;
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
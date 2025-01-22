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
            Debug.Log("EquipNewItem");
            equippedItem = item;
            RequestStateAuthorityForEquipItem(Runner.LocalPlayer);
            RPC_RequestEquipItem(Runner.LocalPlayer);

            currentWieldableIndex = Inventory.instance.selectedItemIndex;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;

            GameObject.Find("CurrentPlayer").GetComponent<FirstPersonOptimizer>().Wield();
            SpawnEquippedItem(player);
        }

        private void RequestStateAuthorityForEquipItem(PlayerRef player)
        {
            if (HasStateAuthority)
            {
                Debug.Log("Already have StateAuthority.");
                return;
            }

            Debug.Log("Requesting StateAuthority for EquipItem.");
            Object.RequestStateAuthority();
            LogStateAuthorityStatus();
        }

        private void LogStateAuthorityStatus()
        {
            string status = HasStateAuthority ? "has" : "does not have";
            Debug.Log($"This client {status} StateAuthority over {gameObject.name}");
        }

        public Transform CurrentWieldableRootTransform()
        {
            if (equippedItem == null || equippedItem.wieldablePrefab == null) return null;

            var prefab = equippedItem.wieldablePrefab;
            bool hasFlashlight = prefab.GetComponent<Flashlight>() != null;
            bool hasConeDetection = prefab.GetComponent<ConeDetection>() != null;

            if (!hasFlashlight && !hasConeDetection)
            {
                GameObject currentPlayer = GameObject.Find("CurrentPlayer");
                return currentPlayer.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (!hasFlashlight && hasConeDetection)
            {
                return cameraPositon;
            }
            else if (hasFlashlight && !hasConeDetection)
            {
                return flashlightPosition;
            }

            return null;
        }

        private void SpawnEquippedItem(PlayerRef player)
{
    Owner = player;

    Transform spawnTransform = CurrentWieldableRootTransform();
    if (spawnTransform == null)
    {
        Debug.LogError($"Unexpected item type: {equippedItem.wieldablePrefab.name}");
        return;
    }

    // 使用世界空间中的身份旋转
    Quaternion spawnRotation = Quaternion.identity;
    
    NetworkObject spawnedItem = Runner.Spawn(
        equippedItem.wieldablePrefab, 
        spawnTransform.position, 
        spawnRotation  // 使用身份旋转
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
    if (Object.HasStateAuthority)
    {
        // 确保在设置父级之前重置物体的本地变换
        spawnedItem.transform.localScale = Vector3.one;
        spawnedItem.transform.SetParent(parent);
        spawnedItem.transform.localPosition = Vector3.zero;
        spawnedItem.transform.localRotation = Quaternion.identity;
        
        RPC_SyncSpawnedItem(spawnedItem.Id, parent.gameObject.name);
    }

            if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
            {
                currentWieldable = wieldable;
                currentWieldable.player = player;
                Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
private void RPC_SyncSpawnedItem(NetworkId itemId, string parentName)
{
    if (!Object.HasStateAuthority && Runner.TryFindObject(itemId, out NetworkObject spawnedItem))
    {
        Transform parent = GameObject.Find(parentName).transform;
        // 确保在设置父级之前重置物体的本地变换
        spawnedItem.transform.localScale = Vector3.one;
        spawnedItem.transform.SetParent(parent);
        spawnedItem.transform.localPosition = Vector3.zero;
        spawnedItem.transform.localRotation = Quaternion.identity;
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
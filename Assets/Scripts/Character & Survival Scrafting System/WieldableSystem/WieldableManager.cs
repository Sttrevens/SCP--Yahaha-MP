using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using System.Collections;
using TMPro;

namespace LPSurvivalEngine
{
    /// <summary>
    /// 处理网络生成，以及游戏世界系统的耦合
    /// </summary>
    public class WieldableManager : NetworkBehaviour
    {
        [Space]
        [Header("Wieldable Manager")]
        [Space]
        [Space]

        public Transform wieldablesPosition;
        public Transform flashlightPosition;
        private string cameraPositonPath = "Model/Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/IndexFinger_01 1/IndexFinger_02 1/IndexFinger_03 1/IndexFinger_04 1";
        public Transform aimPositon;
        
        [SerializeField] private TextMeshProUGUI hintActionText;
        [SerializeField] private PlayerInput playerInput;

        [HideInInspector] public Wieldable currentWieldable;
        [Networked] public PlayerRef Owner { get; set; }

        public ItemDatabase equippedItem;
        public static WieldableManager instance;
        private NetworkObject currentPlayerObject;

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

        void Update()
        {
            if (currentWieldable != null && currentWieldable.altInputHint != null)
            {
                hintActionText.text = string.Format(currentWieldable.altInputHint, "RMB");
                hintActionText.text = hintActionText.text.Replace ("\\n", "\n");  
            }
            else
            {
                hintActionText.text = "";
            }
        }

        private bool IsValidWieldableAction(InputAction.CallbackContext context)
        {
            return context.phase == InputActionPhase.Performed && 
                   currentWieldable != null && 
                   PlayerController.instance.cursor;
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
            RequestStateAuthorityForEquipItem(item);

//if (!Object.HasStateAuthority) RequestStateAuthorityForEquipItem(Runner.LocalPlayer);
            
        }

        private Coroutine requestAuthorityCoroutine;
        private bool isRequestingAuthority = false;

        public void RequestStateAuthorityForEquipItem(ItemDatabase item)
        {
            // 防止重复开启协程
            if (!isRequestingAuthority)
            {
                requestAuthorityCoroutine = StartCoroutine(RequestAuthorityLoop(item));
            }
        }

        private IEnumerator RequestAuthorityLoop(ItemDatabase item)
        {
            isRequestingAuthority = true;

            // 你可以自定义一个上限，防止死循环
            int maxAttempts = 10; 
            int attempts = 0;

            while (!Object.HasStateAuthority && attempts < maxAttempts)
            {
                Object.RequestStateAuthority();
                attempts++;

                // 等待一小段时间再请求下一次
                yield return new WaitForSeconds(0.1f);
            }

            // 请求结束，要么成功，要么超时
            if (Object.HasStateAuthority)
            {
                Debug.Log($"成功拿到 StateAuthority，尝试次数: {attempts}");
                // 在这之后你可以执行真正需要有权限才可以做的操作，比如 EquipNewItem()
                Debug.Log("EquipNewItem");
                    equippedItem = item;
                    RPC_RequestEquipItem(Runner.LocalPlayer);
            }
            else
            {
                Debug.LogWarning($"多次尝试仍未拿到 StateAuthority，尝试次数: {attempts}");
                // 你可以做一些 fallback 处理
            }

            isRequestingAuthority = false;
            requestAuthorityCoroutine = null;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            currentPlayerObject = Runner.GetPlayerObject(Runner.LocalPlayer);
            if (equippedItem.wieldablePrefab.GetComponent<Wieldable>() != null)
                currentPlayerObject.GetComponent<AnimatorManager>().IsHolding = true;
            if (!Object.HasStateAuthority) return;

            //GameObject.Find("CurrentPlayer").GetComponent<FirstPersonOptimizer>().Wield();
            SpawnEquippedItem(player);
            currentWieldableIndex = Inventory.instance.selectedItemIndex;
        }
        
        public Transform CurrentWieldableRootTransform()
        {
            if (equippedItem == null || equippedItem.wieldablePrefab == null) return null;

            var prefab = equippedItem.wieldablePrefab;
            bool hasFlashlight = prefab.GetComponent<Flashlight>() != null;
            bool hasConeDetection = prefab.GetComponent<ConeDetection>() != null;

            GameObject playerObject = GameObject.Find("CurrentPlayer");
            if (playerObject == null) return null;

            if (!hasFlashlight && !hasConeDetection)
            {
                return playerObject.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (!hasFlashlight && hasConeDetection)
            {
                return playerObject.transform.Find(cameraPositonPath);
            }
            else if (hasFlashlight && !hasConeDetection)
            {
                return playerObject.transform.Find("UpperBody/CameraRoot/FlashlightRoot");
            }

            return null;
        }
        
        private void SpawnEquippedItem(PlayerRef player)
        {
            Owner = player;

            /*Transform spawnTransform = CurrentWieldableRootTransform();
            if (spawnTransform == null)
            {
                Debug.LogError($"Unexpected item type: {equippedItem.wieldablePrefab.name}");
                return;
            }*/

            // 使用世界空间中的身份旋转
            Quaternion spawnRotation = Quaternion.identity;
            
            NetworkObject spawnedItem = Runner.Spawn(
                equippedItem.wieldablePrefab, 
                GameObject.Find("CurrentPlayer").transform.Find(cameraPositonPath).position, 
                spawnRotation  // 使用身份旋转
            );

            if (spawnedItem == null)
            {
                Debug.LogError($"Failed to spawn item: {equippedItem.wieldablePrefab.name}");
                return;
            }

            if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
            {
                currentWieldable = wieldable;
                currentWieldable.player = player;
                Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");
            }

            if (spawnedItem.TryGetComponent<CameraController>(out var cameraController))
            {
                cameraController.gameObject.GetComponentInChildren<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("FirstPerson"));
            }
            Debug.Log("现在的相机位置"+cameraPositonPath);
            spawnedItem.transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find(cameraPositonPath));
            spawnedItem.transform.localPosition = Vector3.zero;
            spawnedItem.transform.localRotation = Quaternion.identity;

                    // NetworkObject playerObject = Runner.GetPlayerObject(player);
                    // if (playerObject != null)
                    // {
                    //     MaterialRenderTextureManager.Instance.AssignMaterialAndRenderTexture(playerObject);
                    // }

            //SetupSpawnedItem(spawnedItem, spawnTransform, player);
        }

//         private void SetupSpawnedItem(NetworkObject spawnedItem, Transform parent, PlayerRef player)
// {
//     if (Object.HasStateAuthority)
//     {
//         // 确保在设置父级之前重置物体的本地变换
//         spawnedItem.transform.localScale = Vector3.one;
//         //spawnedItem.transform.SetParent(GameObject.Find("PublicDropBox").transform);
//         spawnedItem.transform.localPosition = Vector3.zero;
//         spawnedItem.transform.localRotation = Quaternion.identity;
        
//         //RPC_SyncSpawnedItem(spawnedItem.Id, parent.gameObject.name);
//     }

//             if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
//             {
//                 currentWieldable = wieldable;
//                 currentWieldable.player = player;
//                 Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");
//             }
//         }

//         [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
// private void RPC_SyncSpawnedItem(NetworkId itemId, string parentName)
// {
//     if (!Object.HasStateAuthority && Runner.TryFindObject(itemId, out NetworkObject spawnedItem))
//     {
//         Transform parent = GameObject.Find("PublicDropBox").transform;
//         // 确保在设置父级之前重置物体的本地变换
//         spawnedItem.transform.localScale = Vector3.one;
//         spawnedItem.transform.SetParent(parent);
//         //spawnedItem.transform.localPosition = Vector3.zero;
//         spawnedItem.transform.localRotation = Quaternion.identity;
//     }
// }
        /// <summary>
        /// 收起物体
        /// </summary>
        /// <param name="player"></param>
        public void DropWieldable(PlayerRef player)
        {
            Debug.Log("正在调用收起物体的逻辑");
            currentPlayerObject.GetComponent<AnimatorManager>().IsHolding = false;
            currentPlayerObject.GetComponent<AnimatorManager>().IsAiming = false;
            if (currentWieldable != null)
            {
            //     NetworkObject playerObject = Runner.GetPlayerObject(player);
            // if (playerObject != null)
            // {
            //     MaterialRenderTextureManager.Instance.ReleaseMaterialAndRenderTexture(playerObject);
            // }
            EmoteController emoteController = currentWieldable.GetComponent<EmoteController>();
            if (emoteController != null)
            {
                emoteController.OnDestroyed();
            }
            Destroy(currentWieldable.gameObject);
            currentWieldable = null;
            }
        }
    }
}
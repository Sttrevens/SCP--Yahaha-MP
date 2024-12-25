using Fusion;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ItemObject : NetworkBehaviour, IInteractable
    {
        [Networked] public bool IsPickedUp { get; set; } // 是否被拾取的网络同步状态
        [Networked] public PlayerRef Owner { get; set; } // 当前拾取物体的玩家
        [Space]
        [Header("Item")]
        [Space]

        public ItemDatabase item;


        public string GetInteractText()
        {
            return string.Format("{0}", item.displayName);
        }

        public void OnInteract()
        {
            // PickupItem pickupItem = GetComponent<PickupItem>();
            // if (pickupItem != null && !pickupItem.IsPickedUp) // 检查物品状态
            // {
            //     Debug.Log("调用物品的拾取方法");
            //     // 调用物品的拾取方法
            //     pickupItem.RPC_OnPickedUp(Object.StateAuthority);
            // }
            // Inventory.instance.AddItem(item);
            // GetInteractText();
            
            if (!IsPickedUp) // 检查物品状态
            {
                Debug.Log("调用物品的拾取方法");
                // 调用物品的拾取方法
                RPC_OnPickedUp(Object.StateAuthority);
            }
            Inventory.instance.AddItem(item);
        }
        
        private void Update()
        {
            if (IsPickedUp)
            {
                // 物体已被拾取，执行隐藏或其他逻辑
                Destroy(gameObject);
            }
        }

        public void PickUp(PlayerRef player)
        {
            IsPickedUp = true;
            Owner = player; // 记录谁拾取了物体
            Debug.Log($"物品被 {player} 拾取");
        }
    
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_OnPickedUp(PlayerRef player)
        {
            // 只有 StateAuthority 可以修改网络状态
            if (Object.HasStateAuthority)
            {
                PickUp(player);
            }
        }
    }
}
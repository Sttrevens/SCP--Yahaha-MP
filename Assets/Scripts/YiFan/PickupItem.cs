using Fusion;
using UnityEngine;

public class PickupItem : NetworkBehaviour
{
    [Networked] public bool IsPickedUp { get; set; } // 是否被拾取的网络同步状态
    [Networked] public PlayerRef Owner { get; set; } // 当前拾取物体的玩家

    public override void Spawned()
    {
        Debug.Log("Spawned PickupItem");
        Object.RequestStateAuthority();
    }

    private void Update()
    {
        if (IsPickedUp)
        {
            // 物体已被拾取，执行隐藏或其他逻辑
            gameObject.SetActive(false);
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
        Debug.Log("hhhhhhhhhhhhhhhhhhh");
        // 只有 StateAuthority 可以修改网络状态
        if (Object.HasStateAuthority)
        {
            PickUp(player);
        }
    }
}
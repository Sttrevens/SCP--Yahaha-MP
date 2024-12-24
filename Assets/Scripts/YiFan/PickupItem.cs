using Fusion;
using LPSurvivalEngine;
using UnityEngine;

public class PickupItem : NetworkBehaviour
{
    // 是否被拾取的网络同步状态
    [Networked, OnChangedRender(nameof(IsPickedUpChanged))] public bool IsPickedUp { get; set; }
    
    void IsPickedUpChanged()
    {
        if (IsPickedUp)
        {
            Debug.Log("OnInteract Invoked");
            // 物体已被拾取，执行隐藏或其他逻辑
            GetComponent<ItemObject>().OnInteract();
        }
    }

    private void Update()
    {
        if (IsPickedUp)
        {
            Debug.Log("OnInteract Invoked");
            // 物体已被拾取，执行隐藏或其他逻辑
            GetComponent<ItemObject>().OnInteract();
        }
    }

    public void PickUp()
    {
        Debug.Log("PickUp Invoked");
        // if (Object.HasStateAuthority) // 确保只有 State Authority 修改状态
        // {
        //     IsPickedUp = true;
        //     Owner = player; // 记录谁拾取了物体
        // }
        IsPickedUp = true;
    }
}
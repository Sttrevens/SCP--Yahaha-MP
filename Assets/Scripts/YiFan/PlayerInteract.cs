using System;
using Fusion;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    public GameObject medicalBox;

    private void Start()
    {
        medicalBox = GameObject.Find("Lobby").transform.Find("LobbyRoom/YiFanMedicalBox").gameObject;
    }

    private void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PickupItem item = medicalBox.GetComponent<PickupItem>();
            if (item != null && !item.IsPickedUp) // 检查物品状态
            {
                Debug.Log("调用物品的拾取方法");
                // 调用物品的拾取方法
                item.RPC_OnPickedUp(Object.StateAuthority);
            }
        }
    }
}
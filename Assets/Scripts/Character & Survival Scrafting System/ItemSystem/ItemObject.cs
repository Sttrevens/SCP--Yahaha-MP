using UnityEngine;

namespace LPSurvivalEngine
{
    public class ItemObject : MonoBehaviour, IInteractable
    {
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
            // gameObject.SetActive(false);
            // Inventory.instance.AddItem(item);
            // GetInteractText();
            // Destroy(gameObject);
        }

    }
}
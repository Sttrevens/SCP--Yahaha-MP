using UnityEngine;

namespace LPSurvivalEngine
{
    public class SPItemObject : MonoBehaviour, IInteractable
    {
        // 移除网络相关属性，改为普通字段
        public bool IsPickedUp { get; set; }
        public GameObject Owner; // 这里改为GameObject类型，用于单机版记录拾取玩家对应的游戏对象，可根据实际情况调整类型

        [Header("Item")]
        public ItemDatabase item;

        public string GetInteractText()
        {
            return string.Format("{0}", item.displayName);
        }

        public void OnInteract()
        {
            if (!IsPickedUp)
            {
                Debug.Log("调用物品的拾取方法");
                PickUp();
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

        public void PickUp()
        {
            IsPickedUp = true;
            Owner = GameObject.FindGameObjectWithTag("Player"); // 假设玩家对象有"Player"标签，可按实际调整
            if (Owner != null)
            {
                Owner.GetComponent<SPAnimatorManager>().PickupCount++;
            }
            Debug.Log($"物品被 {Owner.name} 拾取");
        }
    }
}
using Fusion;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ItemObject : NetworkBehaviour, IInteractable
    {
        [Networked] public bool IsPickedUp { get; set; } // �Ƿ�ʰȡ������ͬ��״̬
        [Networked] public PlayerRef Owner { get; set; } // ��ǰʰȡ��������
        [Space]
        [Header("Item")]
        [Space]

        public ItemDatabase item;
        [SerializeField] private AudioClip pickupSound;
        [Networked] public float currentDurability { get; set; }

        public bool isDisplayedItem = false;

        public string GetInteractText()
        {
            return string.Format("{0}", item.displayName);
        }

        public void OnInteract()
        {
            // PickupItem pickupItem = GetComponent<PickupItem>();
            // if (pickupItem != null && !pickupItem.IsPickedUp) // �����Ʒ״̬
            // {
            //     Debug.Log("������Ʒ��ʰȡ����");
            //     // ������Ʒ��ʰȡ����
            //     pickupItem.RPC_OnPickedUp(Object.StateAuthority);
            // }
            // Inventory.instance.AddItem(item);
            // GetInteractText();
            
            if (!IsPickedUp) // �����Ʒ״̬
            {
                Debug.Log("������Ʒ��ʰȡ����");
                // ������Ʒ��ʰȡ����
                RPC_OnPickedUp(Runner.LocalPlayer);
            }
            Inventory.instance.PickupItem(this);
        }
        
        public override void FixedUpdateNetwork()
        {
            if (IsPickedUp)
            {
                if (!isDisplayedItem)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void PickUp(PlayerRef player)
        {
            IsPickedUp = true;
            Owner = player; // ��¼˭ʰȡ������
            if (Runner.TryGetPlayerObject(player, out var playerObject))
            {
                playerObject.GetComponent<AnimatorManager>().PickupCount++;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.gameObject, pickupSound);
            }
            Debug.Log($"Ʒ {player} ʰȡ");
        }
    
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_OnPickedUp(PlayerRef player)
        {
            // ֻ�� StateAuthority �����޸�����״̬
            //if (Object.HasStateAuthority)
            //{
                PickUp(player);
            //}
        }
    }
}
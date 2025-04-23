using TMPro;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;


public class ShopManager : NetworkBehaviour
{
    public static ShopManager instance;

    public Transform itemListPanel;
    public GameObject itemPrefab;
    public TextMeshProUGUI playerMoneyText;
    public Shop shop;
    
    private ShopItem _selectedItem;
    
    [SerializeField] private AudioClip purchaseSFX;
    [SerializeField] private AudioClip noMoneySFX;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optionally keep it between scenes
        }
    }

    public override void Spawned()
    {
        RPC_LoadShopItems();
    }

    void Update()
    {
        UpdatePlayerMoneyText();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LoadShopItems()
    {
        foreach (Transform child in itemListPanel)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (ShopItem item in shop.availableItems)
        {
            print(item.name);
            GameObject itemObject = Instantiate(itemPrefab, itemListPanel);
            itemObject.name = item.itemName;
            ShopItemInteraction button = itemObject.GetComponent<ShopItemInteraction>();
            button.Setup(item);
        }
    }

    public void BuyItem(ShopItem item)
    {
        _selectedItem = item;
        OnPurchased(Runner.LocalPlayer);
    }
    
    public void OnPurchased(PlayerRef playerRef)
    {
        if (ScoreManager.Instance.revenueRate >= _selectedItem.price)
        {
            ScoreManager.Instance.revenueRate -= _selectedItem.price;
            // 如果需要把物品加到玩家背包（与拾取一样的方法）
            // 可以在这里写与 Inventory.instance.PickupItem(...); 类似的逻辑
            GivePurchasedItemToPlayer(playerRef, _selectedItem);
            Debug.Log($"玩家 {playerRef} 成功购买了物品：{_selectedItem.itemName}");
        }
        else
        {
            AudioManager.Instance.PlaySFX(gameObject, noMoneySFX);
            Debug.Log("Not enough money to buy " + _selectedItem.itemName);
        }
    }

    /// <summary>
    /// 示例：这里演示给玩家背包增加物品（或者做其他“玩家得到物品”的处理）
    /// </summary>
    private void GivePurchasedItemToPlayer(PlayerRef playerRef, ShopItem itemBought)
    {
        // 此处仅作示例，你可以与Inventory系统配合，实现物品加入背包
        if (Runner.TryGetPlayerObject(playerRef, out var playerObject))
        {
            Inventory.instance.PurchaseItem(itemBought.itemdata);
            AudioManager.Instance.PlaySFX(gameObject, purchaseSFX);
        }
    }


    void UpdatePlayerMoneyText()
    {
        playerMoneyText.text = ScoreManager.Instance.revenueRate.ToString("F2");
    }
}
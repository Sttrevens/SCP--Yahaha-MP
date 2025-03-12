using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public Transform itemListPanel;
    public GameObject itemPrefab;
    public TextMeshProUGUI playerMoneyText;
    public int playerMoney = 100;
    public Shop shop;

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

    private void Start()
    {
        UpdatePlayerMoneyText();
        LoadShopItems();
    }

    void LoadShopItems()
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
        if (playerMoney >= item.price)
        {
            playerMoney -= item.price;
            UpdatePlayerMoneyText();
            Debug.Log("Bought " + item.itemName);
        }
        else
        {
            Debug.Log("Not enough money to buy " + item.itemName);
        }
    }

    void UpdatePlayerMoneyText()
    {
        playerMoneyText.text = "Money: " + playerMoney.ToString();
    }
}
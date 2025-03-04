using UnityEngine;
using UnityEngine.UI;
using AB.Shop;


public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public Transform itemListPanel;
    public GameObject itemButtonPrefab;
    public Text playerMoneyText;
    public int playerMoney = 100;

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
        if (Shop.instance == null)
        {
            Debug.LogError("Shop needs to be in.");
            return;
        }

        UpdatePlayerMoneyText();
        LoadShopItems();
    }

    void LoadShopItems()
    {
        foreach (Transform child in itemListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (ShopItem item in Shop.instance.availableItems)
        {
            GameObject itemButton = Instantiate(itemButtonPrefab, itemListPanel);
            ShopItemInteraction button = itemButton.GetComponent<ShopItemInteraction>();
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
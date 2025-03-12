using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemInteraction : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Image itemIconImage;

    private ShopItem shopItem;

    public void Setup(ShopItem item)
    {
        shopItem = item;

        itemNameText.text = item.itemName;
        itemPriceText.text = "Price: " + item.price.ToString();
        itemIconImage.sprite = item.itemIcon;
    }

    public void Buy()
    {
       ShopManager.instance.BuyItem(shopItem);
    }
}

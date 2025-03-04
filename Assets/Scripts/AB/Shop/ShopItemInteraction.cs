using UnityEngine;
using UnityEngine.UI;

public class ShopItemInteraction : MonoBehaviour
{
    public Text itemNameText;
    public Text itemPriceText;
    public Image itemIconImage;

    private ShopItem shopItem;

    public void Setup(ShopItem item)
    {
        shopItem = item;

        itemNameText.text = item.itemName;
        itemPriceText.text = "Price: " + item.price.ToString();
        itemIconImage.sprite = item.itemIcon;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
       ShopManager.instance.BuyItem(shopItem);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Shop", menuName = "Shop/Shop")]
public class Shop : ScriptableObject
{
    public ShopItem[] availableItems;
}
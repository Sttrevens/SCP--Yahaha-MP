
using UnityEngine;
using LPSurvivalEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int price;
    public ItemDatabase itemdata;
}
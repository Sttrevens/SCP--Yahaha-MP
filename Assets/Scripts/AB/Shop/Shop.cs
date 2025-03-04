// Shop.cs
namespace AB.Shop
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Shop", menuName = "Shop/Shop")]
    public class Shop : ScriptableObject
    {
        //singleton shop
        public static Shop instance;
        public ShopItem[] availableItems;

        private void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
    }
}
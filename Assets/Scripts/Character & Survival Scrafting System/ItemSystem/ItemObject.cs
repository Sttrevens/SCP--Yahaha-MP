using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ItemObject : MonoBehaviour, IInteractable
    {
    [Space]
    [Header("Item")]
    [Space]

    public ItemDatabase item;
        public bool isDisplayedItem = false;

    public string GetInteractText()
    {
        return string.Format("{0}", item.displayName);
    }

    public void OnInteract()
    {
        Inventory.instance.AddItem(item);
        GetInteractText();
            if (!isDisplayedItem)
            {
                Destroy(gameObject);
            }
    }

}
}
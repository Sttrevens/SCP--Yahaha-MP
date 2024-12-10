using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class CraftingSystem : MonoBehaviour
    {
    [Space]
    [Header("Crafting System")]
    [Space]

    public CraftingUI[] craftingUI;

    public static CraftingSystem instance;


    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        Inventory.instance.onOpenInventory.AddListener(OnOpenInventory);
    }

    void OnDisable()
    {
        Inventory.instance.onOpenInventory.RemoveListener(OnOpenInventory);
    }
    
    void OnOpenInventory()
    {
        gameObject.SetActive(false);
    }

    public void Craft(CraftingItem craftItem)
    {
        for (int i = 0; i < craftItem.cost.Length; i++)
        {
            for (int x = 0; x < craftItem.cost[i].quantity; x++)
            {
                Inventory.instance.RemoveItem(craftItem.cost[i].item);
            }
        }
        
        Inventory.instance.AddItem(craftItem.itemToCraft);
        
        for (int i = 0; i < craftingUI.Length; i++)
        {
            craftingUI[i].UpdateCanCraft();
        }
        
    }
}


}
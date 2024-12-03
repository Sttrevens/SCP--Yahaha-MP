using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LPSurvivalEngine
{
    public class CraftingUI : MonoBehaviour
    {
    [Space]
    [Header("Crafting UI")]
    [Space]
    [Space]

    [Space]
    [Header("Crafting")]
    [Space]

    public CraftingItem craftItem;
    public Image icon;

    [Space]
    [Space]
    [Header("Craft Price")]
    [Space]
    [Space]

    public Image[] resourceCosts;

    [Space]
    [Header("Text Settings")]
    [Space]

    public TextMeshProUGUI itemname;

    [Space]
    [Space]

    public Color canCraftColor;
    public Color cannotCraftColor;

    private bool canCraft;


    void OnEnable()
    {
        UpdateCanCraft();
    }

    public void UpdateCanCraft()
    {
        canCraft = true;

        for (int i = 0; i < craftItem.cost.Length; i++)
        {
            if(!Inventory.instance.HasItems(craftItem.cost[i].item, craftItem.cost[i].quantity))
            {
                canCraft = false;
                break;
            }
        }
        itemname.color = canCraft ? canCraftColor : cannotCraftColor;     
    }

    private void Start()
    {
        icon.sprite = craftItem.itemToCraft.icon;
        itemname.text = craftItem.itemToCraft.displayName;
        
        for (int i = 0; i < resourceCosts.Length; i++)
        {
            if (i < craftItem.cost.Length)
            {
                resourceCosts[i].gameObject.SetActive(true);
                resourceCosts[i].sprite = craftItem.cost[i].item.icon;
                resourceCosts[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = craftItem.cost[i].quantity.ToString();
            }
            else
            {
                resourceCosts[i].gameObject.SetActive(false);
            }
            
        }
        
    }

    public void OnClickButton()
    {
        if (canCraft)
        {
            CraftingSystem.instance.Craft(craftItem);
        }
    }
    
    
}


}
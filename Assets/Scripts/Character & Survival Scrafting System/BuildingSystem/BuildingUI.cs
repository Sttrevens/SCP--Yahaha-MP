using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LPSurvivalEngine
{
    public class BuildingUI : MonoBehaviour
    {
    [Space]
    [Header("Building UI")]
    [Space]
    [Space]
    
    [Space]
    [Header("Building")]
    [Space]

    public Building building;
    public Image icon;
    
    [Space]
    [Header("Price")]
    [Space]

    public Image[] resourceCosts;

    [Space]
    [Header("Text Settings")]
    [Space]

    public TextMeshProUGUI buildingName;

    [Space]
    [Space]

    public Color canBuildColor;
    public Color cannotBuildColor;

    private bool canBuild;
    
    
    void OnEnable()
    {
        UpdateCanCraft();
    }

    private void Start()
    {
        icon.sprite = building.icon;
        buildingName.text = building.displayName;
        
        for (int x = 0; x < resourceCosts.Length; x++)
        {
            if (x < building.cost.Length)
            {
                resourceCosts[x].gameObject.SetActive(true);
                resourceCosts[x].sprite = building.cost[x].item.icon;
                resourceCosts[x].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                building.cost[x].quantity.ToString();
            }
            else
            {
                resourceCosts[x].gameObject.SetActive(false);
            }
            
        }
    }

    public void UpdateCanCraft()
    {
        canBuild = true;

        for (int x = 0; x < building.cost.Length; x++)
        {
            if(!Inventory.instance.HasItems(building.cost[x].item, building.cost[x].quantity))
            {
                canBuild = false;
                break;
            }
        }
        buildingName.color = canBuild ? canBuildColor : cannotBuildColor;

    }

    public void OnClickButton()
    {
        if (canBuild)
        {
                Debug.Log("fake");
            BuildingSystem.instance.SetNewBuildingRecipe(building);
        }

        /*else
        {
                Debug.Log("fuck");
                PlayerController.instance.ToggleCursor(true);
            BuildingSystem.instance.gameObject.SetActive(false);
        }*/
    }
    
    
}


}
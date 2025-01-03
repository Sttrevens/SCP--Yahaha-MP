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
    [Header("Building")]
    
    public Building building;
    public Image icon;
    
    [Header("Price")]

    public Image[] resourceCosts;

    [Header("Text Settings")]

    public TextMeshProUGUI buildingName;

    [Header("Visible Settings")]
    public bool isUnlocked = false;
    public GameObject ItemPanel;
    public GameObject LockedPanel;
    
    public Color canBuildColor;
    public Color cannotBuildColor;

    private bool canBuild;

    
    void OnEnable()
    {
        LockedPanel.SetActive(!isUnlocked);
        ItemPanel.SetActive(isUnlocked);
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
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LPSurvivalEngine
{
    public class CollectionUI : MonoBehaviour
    {
    [Header("Collection")]
    
    //public Building building;
    public Image icon;

    [Header("Text Settings")]
    public TextMeshProUGUI buildingName;

    [Header("Visible Settings")]
    public bool isUnlocked = false;
    public GameObject ItemPanel;
    public GameObject LockedPanel;

    
    void OnEnable()
    {
        LockedPanel.SetActive(!isUnlocked);
        ItemPanel.SetActive(isUnlocked);
    }

    private void Start()
    {
        // icon.sprite = building.icon;
        // buildingName.text = building.displayName;
        
        // for (int x = 0; x < resourceCosts.Length; x++)
        // {
        //     if (x < building.cost.Length)
        //     {
        //         resourceCosts[x].gameObject.SetActive(true);
        //         resourceCosts[x].sprite = building.cost[x].item.icon;
        //         resourceCosts[x].transform.GetComponentInChildren<TextMeshProUGUI>().text =
        //         building.cost[x].quantity.ToString();
        //     }
        //     else
        //     {
        //         resourceCosts[x].gameObject.SetActive(false);
        //     }
        // }
    }

    
    
}


}

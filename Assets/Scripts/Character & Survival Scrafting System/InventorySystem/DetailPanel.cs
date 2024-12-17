using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailPanel : MonoBehaviour
{
    public TextMeshProUGUI itemNameText; // 名称显示
    //public TextMeshProUGUI itemDescriptionText; // 描述显示


    public void ShowDetail(string itemName)
    {
        itemNameText.text = itemName;
        //itemDescriptionText.text = itemDescription;
    }

}

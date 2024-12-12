using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotInit : MonoBehaviour
{
    [Header("QuickSlot parameters")]
    public GameObject quickSlotPanel;
    public GameObject slotPrefab;

    void Start()
    {
        RectTransform slotPanelRt = quickSlotPanel.GetComponent<RectTransform>();
        GridLayoutGroup slotPanelLg = quickSlotPanel.GetComponent<GridLayoutGroup>();
        int slotCounts = Mathf.FloorToInt(slotPanelRt.rect.width / slotPanelLg.cellSize.x);
        Debug.Log("SlotsCount:"+slotCounts);
        for(int i=1;i<=slotCounts;i++)
        {
            Debug.Log(String.Format("第{0}个slots", i));
            GameObject slot = Instantiate(slotPrefab,slotPanelRt);
            slot.transform.Find("Counts").GetComponent<TextMeshProUGUI>().text = String.Empty;
            slot.transform.Find("KeyboardPrompt").GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }
}

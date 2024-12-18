using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControl : MonoBehaviour
{
    [Header("Panel")]
    public GameObject bagPanel;
    public GameObject buildPanel;
    public GameObject mapPanel;
    public GameObject collectionPanel;

    [Header("Button")]
    public Button bagButton;
    public Button buildButton;
    public Button mapButton;
    public Button collectionButton;


    private void Start()
    {
        // 动态绑定按钮事件
        bagButton.onClick.AddListener(() => SwitchPanel(PanelType.Bag));
        buildButton.onClick.AddListener(() => SwitchPanel(PanelType.Build));
        mapButton.onClick.AddListener(() => SwitchPanel(PanelType.Map));
        collectionButton.onClick.AddListener(() => SwitchPanel(PanelType.Collection));
    }

    void OnEnable()
    {
        bagPanel.SetActive(true);
        buildPanel.SetActive(false);
        mapPanel.SetActive(false);
        collectionPanel.SetActive(false);
    }

     public enum PanelType
    {
        Bag,
        Build,
        Map,
        Collection
    }


    public void SwitchPanel(PanelType panelType)
    {
        bagPanel.SetActive(false);
        buildPanel.SetActive(false);
        mapPanel.SetActive(false);
        collectionPanel.SetActive(false);

        switch (panelType)
        {
            case PanelType.Bag:
                bagPanel.SetActive(true);
                break;
            case PanelType.Build:
                buildPanel.SetActive(true);
                break;
            case PanelType.Map:
                mapPanel.SetActive(true);
                break;
            case PanelType.Collection:
                collectionPanel.SetActive(true);
                break;
        }

    }
}

using LPSurvivalEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class VendingMachineController : MonoBehaviour, IInteractable
{
    [SerializeField] private VendingMachineStuff[] vendingMachineStuffs;

    public ItemDatabase dollarItem;
    public Transform stuffOutputPosition;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetInteractText()
    {
        return "Purchase (2$)(Stock: " + vendingMachineStuffs.Length + ")";
    }

    public void OnInteract()
    {
        // 先确保玩家对象存在且玩家的背包（Inventory）组件存在
        if (player != null && player.GetComponent<Inventory>() != null)
        {
            for (int i = 0; i < player.GetComponent<Inventory>().slots.Length; i++)
            {
                if (player.GetComponent<Inventory>().slots[i].item == dollarItem)
                {
                    if (player.GetComponent<Inventory>().slots[i].quantity >= 2)
                    {
                        if (vendingMachineStuffs != null && vendingMachineStuffs.Length > 0)
                        {
                            GameObject firstStuff = vendingMachineStuffs[0].stuff;
                            Vector3 spawnPosition = stuffOutputPosition.position;
                            GameObject newObject = Instantiate(firstStuff, spawnPosition, Quaternion.identity);
                            player.GetComponent<Inventory>().slots[i].quantity -= 2;
                            if (player.GetComponent<Inventory>().slots[i].quantity == 0)
                                player.GetComponent<Inventory>().slots[i].item = null;
                            player.GetComponent<Inventory>().UpdateUI();
                            Debug.Log("成功生成物品: " + newObject.name);
                        }
                        else
                        {
                            Debug.Log("售货机物品列表为空");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("玩家对象未设置或玩家没有背包（Inventory）组件");
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}

[Serializable]
public class VendingMachineStuff
{
    public GameObject stuff;
    //public int price;
}
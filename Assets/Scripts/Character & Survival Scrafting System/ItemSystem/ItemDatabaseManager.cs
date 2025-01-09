using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager Instance;

    public List<ItemDatabase> allItems; // 全局物品列表

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 根据索引查找物品
    public ItemDatabase GetItemByIndex(int index)
    {
        if (index >= 0 && index < allItems.Count)
        {
            return allItems[index];
        }
        Debug.LogError($"ItemDatabase index out of range: {index}");
        return null;
    }

    // 根据名称查找物品
    public ItemDatabase GetItemByName(string name)
    {
        foreach (var item in allItems)
        {
            if (item.displayName == name)
            {
                return item;
            }
        }
        Debug.LogError($"ItemDatabase with name {name} not found.");
        return null;
    }
}

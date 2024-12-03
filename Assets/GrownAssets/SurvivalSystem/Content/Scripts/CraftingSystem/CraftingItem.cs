using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    [CreateAssetMenu(fileName = "Crafting Item", menuName = "GrownAssets/Create/Crafting Item")]

    public class CraftingItem : ScriptableObject
    {
    [Space]
    [Header("Crafting Item")]
    [Space]

    [Space]
    [Header("Item")]
    [Space]

    public ItemDatabase itemToCraft;

    [Space]
    [Header("Price")]
    [Space]

    public ResourceCost[] cost;
    }

    [System.Serializable]
    public class ResourceCost
    {
        public ItemDatabase item;
        public int quantity;
    }
}
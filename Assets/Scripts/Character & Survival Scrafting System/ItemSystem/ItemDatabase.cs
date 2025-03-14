using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public enum ItemType
    {
    Resource,
    Wieldable,
    Consumable
    }

    public enum ConsumableType
    {
    Hunger,
    Thirst,
    Health,
    Sleep
    }

    public enum PotType
    {
        Pan,
        Saucepot
    }

    [CreateAssetMenu(fileName = "Item",menuName = "GrownAssets/Create/Item")]
    public class ItemDatabase : ScriptableObject
    {
    [Space]
    [Header("Item Object")] 
    [Space]

    public string displayName;
    public string description;

    public ItemType type;

    [Space]

    public Sprite icon;

    [Space]

    public GameObject dropPrefab;

    [Space]
    [Header("Inventory Settings")] 
    [Space]

    public bool canStackItem;
    public int maxStackamount;

    [Space]
    [Header("Consumable")] 
    [Space]

    public ItemDataConsumable[] consumables;

        [Space]
        [Header("Cooking Ingrediant")]
        [Space]

        public bool canBeCooked;
        public float cookTime = 20f;
        public ItemDatabase cookedItem;


        [Space]
    [Header("Wieldable")]
    [Space] 

    public GameObject wieldablePrefab;
    public float maxDurability;

    [Space] [Header("Resource")] [Space]
    
    public ItemDatabase targetItem;

    public int refillDurabilityAmount;

        [Space]
        [Header("Cooking Pot")]
        [Space]

        public bool isPot;
        public PotType potType;
        public AudioClip startCookingSound;
        public AudioClip cookingSound;
        public AudioClip cookedSound;

    }

    [System.Serializable]
    public class ItemDataConsumable
    {
    public ConsumableType type;
    public float value;
    }

}
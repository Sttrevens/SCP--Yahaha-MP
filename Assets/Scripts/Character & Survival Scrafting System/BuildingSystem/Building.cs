using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    [CreateAssetMenu(fileName = "Building", menuName = "GrownAssets/Create/Building")]
    public class Building : ScriptableObject
    {
        [Space]
        [Header("Building")]
        [Space]

        [Space]
        [Header("Assignments")]
        [Space]

        public string displayName;
        public Sprite icon;

        [Space]
        [Header("Building Object")]
        [Space]
        
        public GameObject spawnPrefab;
        public GameObject previewPrefab;

        [Space]
        [Header("Price")]
        [Space]

        public ResourceCost[] cost;
    } 
}

namespace LPSurvivalEngine
{
    public class Buildings : MonoBehaviour {}
}

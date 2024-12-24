using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class CraftBench : Buildings, IInteractable
    {
    [Space]
    [Header("Crafting Bench")]
    [Space]

    public CraftingSystem craftingSystem;
    private PlayerController player;

        [SerializeField] private string objectName;

    private void Start()
    {
            if (craftingSystem == null)
            {
                craftingSystem = player.gameObject.GetComponent<CraftingSystem>();
            }
        player = FindObjectOfType<PlayerController>();
    }

    public string GetInteractText()
    {
        return "Crafting Bench";
    }

    public void OnInteract()
    {
        craftingSystem.gameObject.SetActive(true);
        player.ToggleCursor(true);
    }

        public string GetObjectName()
        {
            return objectName;
        }

        public void SetPlayer(PlayerController player)
        {
            this.player = player;
        }    
}

}


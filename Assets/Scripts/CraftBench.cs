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
    

    private void Start()
    {
        craftingSystem = FindObjectOfType<CraftingSystem>(true);
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
}

}


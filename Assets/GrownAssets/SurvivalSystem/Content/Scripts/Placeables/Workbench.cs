using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class Workbench : Buildings, IInteractable
    {
    [Space]
    [Header("Crafting Table")]
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
        return "Crafting Table";
    }

    public void OnInteract()
    {
        craftingSystem.gameObject.SetActive(true);
        player.ToggleCursor(true);
    }
}

}


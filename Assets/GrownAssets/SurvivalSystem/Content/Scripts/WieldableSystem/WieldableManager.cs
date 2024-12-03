using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LPSurvivalEngine
{
    public class WieldableManager : MonoBehaviour
    {  
    [Space]
    [Header("Wieldable Manager")]
    [Space]
    [Space]
    
    public Wieldable currentWieldable;
    public Transform wieldablesPosition;
    
    public static WieldableManager instance;
    private PlayerController controller;


    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
        {
            currentWieldable.OnAttackInput();
        }
    }
    
    public void OnAltAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
        {
            currentWieldable.OnAltAttackInput();
        }
    }

    public void EquipNewItem(ItemDatabase item)
    {
        DropWieldable();
        currentWieldable = Instantiate(item.wieldablePrefab, wieldablesPosition).GetComponent<Wieldable>();
    }

    public void DropWieldable()
    {
        if (currentWieldable != null)
        {
            Destroy(currentWieldable.gameObject);
            currentWieldable = null;
        }
    }
    
    
}


}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public Transform flashlightPosition;
    
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
            if (item.wieldablePrefab.GetComponent<Flashlight>() == null)
            {
                currentWieldable = Instantiate(item.wieldablePrefab, wieldablesPosition).GetComponent<Wieldable>();
            }
            else
            {
                currentWieldable = Instantiate(item.wieldablePrefab, flashlightPosition).GetComponent<Wieldable>();
            }
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
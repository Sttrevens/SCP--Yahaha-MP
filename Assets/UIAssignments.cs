using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIAssignments : MonoBehaviour
{
    public Transform dropPosition;

    public PlayerController playerController;

    public Transform wieldPostition;
    public Transform flashlightRootPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        Inventory.instance.dropPosition = dropPosition;
        Inventory.instance.playerController = playerController;
        Inventory.instance.vitals = GetComponent<HealthSystem>();

        ExitMenu.instance.playerController = playerController;

        WieldableManager.instance.wieldablesPosition = wieldPostition;
        WieldableManager.instance.flashlightPosition = flashlightRootPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

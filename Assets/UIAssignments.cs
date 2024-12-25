using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIAssignments : MonoBehaviour
{
    public Transform dropPosition;

    public PlayerController playerController;
    public PlayerInput playerInput;
    // Start is called before the first frame update
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerInput = GameObject.Find("InputManager").GetComponent<PlayerInput>();
    }

    void Start()
    {
        Inventory.instance.dropPosition = dropPosition;
        Inventory.instance.playerController = playerController;
        Inventory.instance.PlayerInput = playerInput;
        Inventory.instance.vitals = GetComponent<HealthSystem>();

        Prompt.instance.playerInput = playerInput;

        ExitMenu.instance.playerController = playerController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

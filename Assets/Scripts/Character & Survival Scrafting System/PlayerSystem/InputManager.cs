using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace LPSurvivalEngine
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        
        public bool Crouch { get; private set; }
        public bool AttackTwoHand { get; private set; }
        public bool AttackOneHand { get; private set; }
        public int SlotIndex { get; private set; }

        private InputActionMap currentMap;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction jumpAction;
        private InputAction slotSelectAction;
        private InputAction attackActionTwoHand;
        private InputAction attackActionOneHand;

        private InputAction crouchAction;
        private InputAction dropAction;
        private InputAction useAction;
        private InputAction exitAction;

        public bool SendDamage;

        private void Awake()
        {
            currentMap = PlayerInput.currentActionMap;

            moveAction = currentMap.FindAction("Move");
            lookAction = currentMap.FindAction("Look");
            runAction = currentMap.FindAction("Run");
            jumpAction = currentMap.FindAction("Jump");
            slotSelectAction = currentMap.FindAction("SelectSlot");
            attackActionTwoHand = currentMap.FindAction("TwoHandAttack");
            attackActionOneHand = currentMap.FindAction("OneHandAttack");
            crouchAction = currentMap.FindAction("Crouch");
            dropAction = currentMap.FindAction("Drop");
            useAction = currentMap.FindAction("Action");
            exitAction = currentMap.FindAction("Escape");

            moveAction.performed += onMove;
            lookAction.performed += onLook;
            runAction.performed += onRun;
            jumpAction.performed += onJump;
            slotSelectAction.canceled += OnSelectSlot;

            dropAction.started += OnDrop;
            useAction.started += OnUse;
            exitAction.started += OnExit;
            crouchAction.performed += onCrouch;
            crouchAction.canceled += onCrouch;

            moveAction.canceled += onMove;
            lookAction.canceled += onLook;
            runAction.canceled += onRun;
            jumpAction.canceled += onJump;
        }

        //Gameplay related
        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void onJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }

        private void onCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.ReadValueAsButton();
        }

        private void OnSelectSlot(InputAction.CallbackContext context)
        {
            string keypressed = context.control.displayName;

            if (keypressed == "0") SlotIndex = 9;
            else SlotIndex = int.Parse(keypressed) - 1;

            Inventory.instance.SelectItem(SlotIndex);
        }

        private void OnUse(InputAction.CallbackContext context)
        {
            Inventory.instance.UseItem();
        }

        private void OnDrop(InputAction.CallbackContext context)
        {
            Inventory.instance.DropItem();    
        }

        void AttackEvent()
        {
            SendDamage = true;
        }

        //Menu Related
        private void OnExit(InputAction.CallbackContext context)
        {
            if (!ExitMenu.instance.isPaused)
            {
            Debug.Log("Click Escape");
            ExitMenu.instance.ShowExitMenu();
                ExitMenu.instance.isPaused = true;
        }
            else
            {
                ExitMenu.instance.HideExitMenu();
            }
        }

        private void OnEnable()
        {
            currentMap.Enable();
        }

        private void OnDisable()
        {
            currentMap.Disable();
        }
    }
}
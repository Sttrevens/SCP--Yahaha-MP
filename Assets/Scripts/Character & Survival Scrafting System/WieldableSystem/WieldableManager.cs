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
        public PlayerInput PlayerInput;
        private InputAction actionAction;
    
        public static WieldableManager instance;
        public PlayerController controller;


        private void Awake()
        {
            instance = this;
            controller = GetComponent<PlayerController>();
            // PlayerInput = GameObject.Find("InputManager").GetComponent<PlayerInput>();
            
            if (PlayerInput != null) {
                actionAction = PlayerInput.actions.FindAction("Action");
                actionAction.performed += OnAttackInput;
            }
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
                GameObject player = GameObject.Find("CurrentPlayer");
                wieldablesPosition = player.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
                currentWieldable = Instantiate(item.wieldablePrefab, wieldablesPosition).GetComponent<Wieldable>();
                Debug.Log("EquipNewItem : " + item.wieldablePrefab.name);
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
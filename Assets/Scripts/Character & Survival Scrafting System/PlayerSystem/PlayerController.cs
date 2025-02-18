using UnityEngine;

namespace LPSurvivalEngine
{
    public class PlayerController : MonoBehaviour
    {
        [Space]
        [Header("Player Settings")]
        [Space]

        public static PlayerController instance;
        private Rigidbody rig;
        private InputManager inputManager;
        [HideInInspector] public bool cursor = true;

        private void Awake()
        {
            instance = this;
        }

        public void ToggleCursor(bool toggle)
        {
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            cursor = !toggle;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            Inventory.instance.playerController = this;
            ExitMenu.instance.playerController = this;
            WieldableManager.instance.controller = this;

            Spawn(GameObject.Find("SpawnPoint").transform);

            // hasAnimator = TryGetComponent<Animator>(out animator);
            // rig = GetComponent<Rigidbody>();
            // inputManager = GetComponent<InputManager>();
            //
            // jumping = Animator.StringToHash("Jump");
            // grounding = Animator.StringToHash("Grounded");
        }
        
        public void Spawn(Transform spawnTransform)
        {
            transform.position = spawnTransform.position;
            transform.rotation = spawnTransform.rotation;
        }
    }
}
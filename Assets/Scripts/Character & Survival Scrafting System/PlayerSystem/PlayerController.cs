using UnityEngine;

namespace LPSurvivalEngine
{
    public class PlayerController : MonoBehaviour
    {
        [Space]
        [Header("Player Controller")]
        [Space]
        
        [Space]
        [Header("Player Settings")]
        [Space]

        [SerializeField] private float DistanceGround = 0.8f;

        [Space]

        [SerializeField] private LayerMask GroundCheck;

        [Space]

        public static PlayerController instance;
        private float AnimBlendSpeed = 12f;
        private Rigidbody rig;
        private InputManager inputManager;
        private Animator animator;
        private bool grounded = false;
        private bool hasAnimator;
        private int jumping;
        private int grounding;
        [HideInInspector] public bool cursor = true;

        private bool isAttacking = false;

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

        private void SetAnimationGrounding()
        {
            animator.ResetTrigger(jumping);
            animator.SetBool(grounding, grounded);
        }
        
        public void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        public void Spawn(Transform spawnTransform)
        {
            transform.position = spawnTransform.position;
            transform.rotation = spawnTransform.rotation;
        }
    }
}
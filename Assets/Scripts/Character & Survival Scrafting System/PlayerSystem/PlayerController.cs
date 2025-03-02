using UnityEngine;
using UnityEngine.SceneManagement;

namespace LPSurvivalEngine
{
    public class PlayerController : MonoBehaviour
    {
        [Space]
        [Header("Player Settings")]
        [Space]

        public static PlayerController instance;
        [HideInInspector] public bool cursor = true;

        private void Awake()
        {
            instance = this;
        }

        public void ToggleCursor(bool toggle)
        {
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            cursor = !toggle;
            Cursor.visible = toggle;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
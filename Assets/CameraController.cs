using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

namespace LPSurvivalEngine
{
    /// <summary>
    /// 整个类用来控制相机的左右键逻辑 开镜使用的是硬编码进行的绑定
    /// </summary>
    public class CameraController : Wieldable
    {
        [Header("")]
        [SerializeField] private Light cameraFlashLight;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip toggleSound;
        [SerializeField] private Camera CameraInCamera;

        private Camera mainCamera;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        public float ZoomSpeed = 2.0f;
        public float MinFOV = 20f;
        public float MaxFOV = 60f;

        [SerializeField]private bool isRightMouseButtonDown = false; // 标识右键是否按下

        private void Awake()
        {
            CameraInCamera = GetComponentInChildren<Camera>();
            mainCamera = Camera.main;
            if (cameraFlashLight == null)
            {
                cameraFlashLight = GetComponentInChildren<Light>();
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            Debug.Log("[CameraController] Awake - Components Initialized");
        }

        private void Update()
        {
            if (isRightMouseButtonDown)
            {
                HandleZoom();
            }
        }

        /// <summary>
        /// 拍照逻辑
        /// </summary>
        public override void OnAttackInput()
        {
            Debug.Log("[CameraController] OnAttackInput - Taking Picture");
            TakePicture();
        }

        /// <summary>
        /// 瞄准
        /// </summary>
        public override void OnAltAttackInput()
        {
            isRightMouseButtonDown = !isRightMouseButtonDown;
            Debug.Log("[CameraController] OnAltAttackInput - Aim State: " + isRightMouseButtonDown);
            Aim();
        }

        // 拍照功能（左键）
        void TakePicture()
        {
            Debug.Log("[CameraController] TakePicture - Flash On");
            if (cameraFlashLight != null)
            {
                cameraFlashLight.enabled = true;
            }

            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
                Debug.Log("[CameraController] TakePicture - Playing Sound");
            }

            if (cameraFlashLight != null)
            {
                StartCoroutine(DisableFlashLight());
            }
        }

        // 关闭闪光灯的协程
        IEnumerator DisableFlashLight()
        {
            yield return new WaitForSeconds(0.1f);
            if (cameraFlashLight != null)
            {
                cameraFlashLight.enabled = false;
            }
            Debug.Log("[CameraController] DisableFlashLight - Flash Off");
        }

        void Aim()
        {
            if (isRightMouseButtonDown)
            {
                transform.position = WieldableManager.instance.AimPositon.position;
                Debug.Log("[CameraController] Aim - Aiming at position: " + transform.position);
            }
            else
            {
                transform.position = WieldableManager.instance.cameraPositon.position;
                Debug.Log("[CameraController] Aim - Reset to normal position: " + transform.position);
            }
        }

        void HandleZoom()
        {
            float ScrollInput = InputManager.Instance.Scroll.y;
            Debug.Log("[CameraController] HandleZoom - Scroll Input: " + ScrollInput);
            if (ScrollInput != 0)
            {
                float newFOV = CameraInCamera.fieldOfView - (ScrollInput * ZoomSpeed);
                CameraInCamera.fieldOfView = Mathf.Clamp(newFOV, MinFOV, MaxFOV);
                Debug.Log("[CameraController] HandleZoom - New FOV: " + CameraInCamera.fieldOfView);
            }
        }
    }
}

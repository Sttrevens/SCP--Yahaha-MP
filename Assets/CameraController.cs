using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // 引入 Fusion 命名空间
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

        private Vector3 aimPosition;
        private Quaternion aimRotation;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;

        [SerializeField] private bool isRightMouseButtonDown = false; // 标识右键是否按下

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

            aimPosition = WieldableManager.instance.AimPositon.position;
            aimRotation = WieldableManager.instance.AimPositon.rotation;
            cameraPosition = WieldableManager.instance.cameraPositon.position;
            cameraRotation = WieldableManager.instance.cameraPositon.rotation;
        }

        /// <summary>
        /// 拍照逻辑
        /// </summary>
        public override void OnAttackInput()
        {
            Debug.Log("[CameraController] OnAttackInput - Taking Picture");
            // 使用 RPC 方法同步拍照逻辑
            TakePictureRPC();
        }

        /// <summary>
        /// 瞄准
        /// </summary>
        public override void OnAltAttackInput()
        {
            isRightMouseButtonDown = !isRightMouseButtonDown;
            Debug.Log("[CameraController] OnAltAttackInput - Aim State: " + isRightMouseButtonDown);
            //Aim();
        }

        // 拍照功能（左键）
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        void TakePictureRPC()
        {
            Debug.Log("[CameraController] TakePictureRPC - Flash On");
            if (cameraFlashLight != null)
            {
                cameraFlashLight.enabled = true;
            }

            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
                Debug.Log("[CameraController] TakePictureRPC - Playing Sound");
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
                transform.position = aimPosition;
                transform.rotation = aimRotation;
                Debug.Log("[CameraController] Aim - Aiming at position: " + transform.position);
            }
            else
            {
                transform.position = cameraPosition;
                transform.rotation = cameraRotation;
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

        public override void FixedUpdateNetwork()
        {
            if (isRightMouseButtonDown)
            {
                transform.position = aimPosition;
                transform.rotation = aimRotation;
                Debug.Log("[CameraController] Aim - Aiming at position: " + transform.position);
            }
            else
            {
                transform.position = cameraPosition;
                transform.rotation = cameraRotation;
                Debug.Log("[CameraController] Aim - Reset to normal position: " + transform.position);
            }
        }
    }
}

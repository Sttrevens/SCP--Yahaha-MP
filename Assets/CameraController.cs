using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // ���� Fusion �����ռ�
using LPSurvivalEngine;

namespace LPSurvivalEngine
{
    /// <summary>
    /// ����������������������Ҽ��߼� ����ʹ�õ���Ӳ������еİ�
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

        [SerializeField] private bool isRightMouseButtonDown = false; // ��ʶ�Ҽ��Ƿ���

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

            //aimPosition = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/AimRoot").transform.position;
            //aimRotation = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/AimRoot").transform.rotation;
            //cameraPosition = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/HoldCameraRoot").transform.position;
            //cameraRotation = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/HoldCameraRoot").transform.rotation;
        }

        /// <summary>
        /// �����߼�
        /// </summary>
        public override void OnAttackInput()
        {
            Debug.Log("[CameraController] OnAttackInput - Taking Picture");
            // ʹ�� RPC ����ͬ�������߼�
            TakePictureRPC();
        }

        /// <summary>
        /// ��׼
        /// </summary>
        public override void OnAltAttackInput()
        {
            isRightMouseButtonDown = !isRightMouseButtonDown;
            Debug.Log("[CameraController] OnAltAttackInput - Aim State: " + isRightMouseButtonDown);
            Aim();
        }

        // ���չ��ܣ������
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

        // �ر�����Ƶ�Э��
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
                transform.position = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/AimRoot").transform.position;
                transform.rotation = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/AimRoot").transform.rotation;
                Debug.Log("[CameraController] Aim - Aiming at position: " + transform.position);
            }
            else
            {
                transform.position = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/HoldCameraRoot").transform.position;
                transform.rotation = GameObject.Find("CurrentPlayer").transform.Find("CameraRoot/HoldCameraRoot").transform.rotation;
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

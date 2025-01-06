using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

namespace LPSurvivalEngine
{
    public class CameraController : Wieldable
    {
        [Header("")]
        [SerializeField] private Light cameraFlashLight;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip toggleSound;

        private Camera mainCamera;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private bool isRightMouseButtonDown = false; // 标识右键是否按下

        private void Awake()
        {
            mainCamera = Camera.main;
            if (cameraFlashLight == null)
            {
                cameraFlashLight = GetComponentInChildren<Light>();
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        public override void OnAttackInput()
        {
            TakePicture();
        }

        public override void OnAltAttackInput()
        {
            isRightMouseButtonDown = !isRightMouseButtonDown;
            Aim();
        }

        // 拍照功能（左键）
        void TakePicture()
        {
            // 闪光灯开启
            if (cameraFlashLight != null)
            {
                cameraFlashLight.enabled = true;
            }

            // 播放拍照音效
            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }

            // 等待一小段时间后关闭闪光灯
            if (cameraFlashLight != null)
            {
                StartCoroutine(DisableFlashLight());
            }
        }

        // 关闭闪光灯的协程
        IEnumerator DisableFlashLight()
        {
            yield return new WaitForSeconds(0.1f); // 闪光灯持续时间
            cameraFlashLight.enabled = false;
        }
        void Aim()
        {
            if (isRightMouseButtonDown)
            {
                transform.position = WieldableManager.instance.AimPositon.position;
            }
            else
            {
                transform.position = WieldableManager.instance.cameraPositon.position;
            }
        }
    }
}

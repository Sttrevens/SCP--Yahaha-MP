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

        void Update()
        {
            // 左键拍照
            if (Input.GetMouseButtonDown(0)) // 0 是鼠标左键
            {
                TakePicture();
            }

            // 右键按下时将相机移到主相机前
            if (Input.GetMouseButtonDown(1)) // 1 是鼠标右键
            {
                isRightMouseButtonDown = true;
                SaveOriginalCameraState();
                MoveCameraToMainCamera();
            }

            // 右键松开时恢复相机到原来的状态
            if (Input.GetMouseButtonUp(1)) // 1 是鼠标右键
            {
                isRightMouseButtonDown = false;
                RestoreOriginalCameraState();
            }
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

        // 保存原来的相机位置和旋转
        void SaveOriginalCameraState()
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        // 恢复原来的相机位置和旋转
        void RestoreOriginalCameraState()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }

        // 将相机移到主相机前
        void MoveCameraToMainCamera()
        {
            if (mainCamera != null)
            {
                // 这里简单地将当前相机的位置设置为主相机的位置
                transform.position = mainCamera.transform.position + new Vector3(0, -0.1f, 0.1f);
                transform.rotation = mainCamera.transform.rotation;
            }
        }
    }
}

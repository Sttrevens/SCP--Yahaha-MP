using DestroyIt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class Flashlight : Wieldable
    {
        [Space]
        [Header("Wieldable Flashlight")]
        [Space]

        [SerializeField] private Light torchLight;  // 手电筒光源
        [SerializeField] private AudioSource audioSource;  // 音效播放源
        [SerializeField] private AudioClip toggleSound;  // 音效剪辑

        [Space]
        [Header("Flashlight Position")]
        [Space]

        [SerializeField] private Vector3 offset = new Vector3(0, 0, 2);  // 偏移量，用来调整手电筒相对摄像机的位置

        private Camera cam;

        private bool isTorchOn = false;

        private void Awake()
        {
            cam = Camera.main;
            if (torchLight == null)
                torchLight = GetComponentInChildren<Light>();  // 如果没有指定光源，自动查找子物体中的光源

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();  // 如果没有指定音源，自动获取 AudioSource 组件
        }

        public override void OnAttackInput()
        {
            ToggleFlashlight();
        }

        private void ToggleFlashlight()
        {
            isTorchOn = !isTorchOn;  // 切换手电筒状态

            torchLight.enabled = isTorchOn;  // 设置光源的开启/关闭

            // 播放开关音效
            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
        }
    }
}

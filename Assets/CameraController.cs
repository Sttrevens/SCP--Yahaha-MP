using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // ���� Fusion �����ռ�
using LPSurvivalEngine;
using TMPro;
using UnityEngine.Serialization;

namespace LPSurvivalEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraController : Wieldable
    {
        [Header("Camera")]
        [SerializeField] private Light cameraFlashLight;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip toggleSound;
        [SerializeField] private Camera CameraInCamera;
        [SerializeField] private AudioClip zoomSound;

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

        public bool isRightMouseButtonDown = false;

        [Header("Durability Settings")]
        [SerializeField] private float durabilityDrainPerSecond = 0.2f;

        [Header("UI")]
        [SerializeField] private GameObject[] batteryIcons = new GameObject[3]; // 三个电池图标
        [FormerlySerializedAs("bobojian")] [SerializeField] private string bobojianReferenceinScene;

        [Header("MaterialRenderTextureManager")]
        [SerializeField] private Renderer screenRenderer;

        [Header("Input")]
        public string altInputHint;

        private void DrainDurability()
        {
            // 通过 WieldableManager 获取当前装备的物品槽
            ItemSlot currentSlot = WieldableManager.instance.GetCurrentWieldableSlot();
            if (currentSlot != null)
            {
                // 计算这一帧要消耗的耐久度
                float drainAmount = durabilityDrainPerSecond * Time.fixedDeltaTime;
                
                // 通过 Inventory 更新耐久度
                Inventory.instance.UpdateItemDurability(WieldableManager.instance.GetCurrentWieldableIndex(), drainAmount);

                    // 更新电池图标显示
                    UpdateBatteryIcons(currentSlot.currentDurability);

                    if (currentSlot.currentDurability <= 0)
                    {
                        Debug.Log("[CameraController] Camera battery depleted!");
                        CameraInCamera.enabled = false;
                    }
                    else
                    {
                        CameraInCamera.enabled = true;
                    }
            }
        }

        private void UpdateBatteryIcons(float currentDurability)
        {
            if (batteryIcons.Length == 3)
            {
                // 根据电量显示/隐藏图标
                batteryIcons[0].SetActive(currentDurability > 66);
                batteryIcons[1].SetActive(currentDurability > 33);
                batteryIcons[2].SetActive(currentDurability > 0);
            }
        }

        private void Awake()
        {
            //CameraInCamera = GetComponentInChildren<Camera>();
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

        public override void Spawned()
        {
            if (GameObject.Find(bobojianReferenceinScene) != null)
            {
                batteryIcons[0] = GameObject.Find(bobojianReferenceinScene).transform.Find("BatteryIcon/BatteryContent1").gameObject;
                batteryIcons[1] = GameObject.Find(bobojianReferenceinScene).transform.Find("BatteryIcon/BatteryContent2").gameObject;
                batteryIcons[2] = GameObject.Find(bobojianReferenceinScene).transform.Find("BatteryIcon/BatteryContent3").gameObject;
            }

            if (HasStateAuthority)
            {
                transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot"));
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)  // 确保只在有状态权限的客户端上更新
        {
            // 消耗耐久度
            DrainDurability();

            if (isRightMouseButtonDown)
            {
                HandleZoom();
            }
            Transform topParent = gameObject.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }
            PlayerMovement plMovement = topParent.GetComponent<PlayerMovement>();
            if (plMovement != null)
                plMovement.isAiming = isRightMouseButtonDown;

            if (HasStateAuthority)
            {
                transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot"));
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
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
                // transform.position = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/AimRoot").transform.position;
                // transform.rotation = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/AimRoot").transform.rotation;
                // Debug.Log("[CameraController] Aim - Aiming at position: " + transform.position);

                GetComponent<NetworkMecanimAnimator>().SetTrigger("Aim");
            }
            else
            {
                // transform.position = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot").transform.position;
                // transform.rotation = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot").transform.rotation;
                // Debug.Log("[CameraController] Aim - Reset to normal position: " + transform.position);

                GetComponent<NetworkMecanimAnimator>().SetTrigger("CancelAim");
            }
        }

        private float lastSoundTime = 0f;

        private float lastScrollInput = 0f;
        private float previousScrollTime = 0f;

void HandleZoom()
{
    float ScrollInput = InputManager.Instance.Scroll.y;
    if (ScrollInput != 0)
    {
        // 使用 Lerp 实现平滑缩放
        float targetFOV = CameraInCamera.fieldOfView - (ScrollInput * ZoomSpeed);
        targetFOV = Mathf.Clamp(targetFOV, MinFOV, MaxFOV);
        CameraInCamera.fieldOfView = Mathf.Lerp(CameraInCamera.fieldOfView, targetFOV, Time.fixedDeltaTime * 10f);

        // 声音播放逻辑保持不变
        if (ScrollInput != lastScrollInput && Time.time - lastSoundTime >= 0.3f)
        {
            AudioManager.Instance.PlaySFX(this.gameObject, zoomSound, 0.3f);
            lastScrollInput = ScrollInput;
            lastSoundTime = Time.time;
        }
    }
    else
    {
        lastScrollInput = 0f;
    }
}

public void SetMaterialAndRenderTexture(Material material, RenderTexture renderTexture)
    {
        if (screenRenderer != null)
        {
            screenRenderer.material = material;
        }
        if (CameraInCamera != null)
        {
            CameraInCamera.targetTexture = renderTexture;
        }
        }
    }
}

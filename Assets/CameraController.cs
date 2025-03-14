using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // ���� Fusion �����ռ�
using System.Linq;

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

        [SerializeField] private GameObject[] diedObjects;

        [Header("UI")]
        [SerializeField] private GameObject[] batteryIcons = new GameObject[3]; // 三个电池图标
        public string bobojianReferenceinScene = "";

        [Header("MaterialRenderTextureManager")]
        [SerializeField] private Renderer screenRenderer;

        [Header("Input")]
        public string altInputHint;
        
        /// <summary>
        /// 可用名称池
        /// </summary>
        [SerializeField] private string[] candidateStrings = new string[4]
        {
            "Bobojian",
            "Bobojian 1",
            "Bobojian 2",
            "Bobojian 3"
        };

        /// <summary>
        /// 每个名称对应的 RenderTexture（与 candidateStrings 顺序一一对应）
        /// </summary>
        [SerializeField] private RenderTexture[] candidateRenderTextures = new RenderTexture[4];

        /// <summary>
        /// 每个名称对应的 Material（与 candidateStrings 顺序一一对应）
        /// </summary>
        [SerializeField] private Material[] candidateMaterials = new Material[4];


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
                        foreach (var go in diedObjects)
                        {
                            go.SetActive(false);
                        }
                    }
                    else
                    {
                        foreach (var go in diedObjects)
                        {
                            go.SetActive(true);
                        }
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
            
             // 1. 找出所有已存在的 CameraController，收集它们用的 bobojianReferenceinScene
            CameraController[] allCameras = FindObjectsOfType<CameraController>();
            HashSet<string> usedStrings = new HashSet<string>();
            foreach (var cam in allCameras)
            {
                // 如果它不是自己，并且它的字符串是候选池里的某一个，则算“已被使用”
                if (cam != this && candidateStrings.Contains(cam.bobojianReferenceinScene))
                {
                    usedStrings.Add(cam.bobojianReferenceinScene);
                }
            }

            // 2. 遍历 candidateStrings，找到一个还没被用的字符串
            string chosenString = null;
            int chosenIndex = -1;
            for (int i = 0; i < candidateStrings.Length; i++)
            {
                if (!usedStrings.Contains(candidateStrings[i]))
                {
                    chosenString = candidateStrings[i];
                    chosenIndex = i;
                    break;
                }
            }

            // 3. 如果找到可用字符串，就赋值到 bobojianReferenceinScene，
            //    并从相应列表里获取对应的 RenderTexture 和 Material
            if (!string.IsNullOrEmpty(chosenString) && chosenIndex >= 0)
            {
                bobojianReferenceinScene = chosenString;
                
                if (chosenIndex < candidateRenderTextures.Length 
                    && chosenIndex < candidateMaterials.Length)
                {
                    // 设置 Camera 的输出纹理
                    CameraInCamera.targetTexture = candidateRenderTextures[chosenIndex];
                    // 设置显示屏的材质
                    if (screenRenderer != null)
                    {
                        screenRenderer.material = candidateMaterials[chosenIndex];
                    }
                }

                Debug.Log($"[CameraController] 绑定到可用字符串: {bobojianReferenceinScene}");
            }
            else
            {
                // 如果没有可用字符串（都被占用了），可以根据需求做一定的处理
                // 例如：给一个默认名称，或者提示报错
                Debug.LogWarning("[CameraController] 没有可用的字符串可以使用，可能超过了 4 台摄像机限制。");
            }

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
                transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find("Model/Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/PadHandle"));
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

                GameObject.Find("CurrentPlayer").GetComponent<AnimatorManager>().IsAiming = true;
            }
            else
            {
                // transform.position = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot").transform.position;
                // transform.rotation = GameObject.Find("CurrentPlayer").transform.Find("UpperBody/CameraRoot/HoldCameraRoot").transform.rotation;
                // Debug.Log("[CameraController] Aim - Reset to normal position: " + transform.position);

                GameObject.Find("CurrentPlayer").GetComponent<AnimatorManager>().IsAiming = false;
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

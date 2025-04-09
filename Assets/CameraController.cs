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
        public float baseFOV = 60f;

        private Vector3 aimPosition;
        private Quaternion aimRotation;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;

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
        
        private RigController _rigController;
        private PlayerMovement _playerMovement;
        private float _originalAimSpeed;

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

            _rigController = GameObject.Find("CurrentPlayer").transform.Find("Model").GetComponent<RigController>();
            _playerMovement = GameObject.Find("CurrentPlayer").GetComponent<PlayerMovement>();
            _originalAimSpeed = _playerMovement.aimSpeed;
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

            if (GameObject.Find("CurrentPlayer").GetComponent<AnimatorManager>().IsAiming)
            {
                HandleZoom();
            }
            Transform topParent = gameObject.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }

            if (HasStateAuthority)
            {
                /*if (_rigController.rigs["AimRig"].weight == 1)
                {
                    transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find("AimTargetForPad/PadHandle"));
                }
                else
                {*/
                    transform.SetParent(GameObject.Find("CurrentPlayer").transform.Find("Model/Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/PadHandle"));
                //}
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                
                /*if (transform.root.gameObject.name == "CurrentPlayer")
                {
                    foreach (Transform child in transform)
                    {
                        if (child.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
                        {
                            MeshRenderer childRenderer = child.gameObject.GetComponent<MeshRenderer>();
                            if (childRenderer != null && childRenderer.enabled)
                            {
                                childRenderer.enabled = false;
                            }
                        }
                    }
                }*/
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
            if (!GameObject.Find("CurrentPlayer").GetComponent<AnimatorManager>().IsAiming)
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

        public float soundVolume = 0.3f;    // 声音音量
        public float minSoundInterval = 0.3f;   // 两次声音播放的最小间隔
        public float scrollStep = 0.1f;   // 定义每经过多少累计滚动单位播放一次声音
        private int lastZoomStep = 0;
        private float lastSoundTime = 0f;
        
        // 累计的滚轮输入量
        private float accumulatedScroll = 0f;

        private bool pendingSwitchState = false;

void HandleZoom()
{
    // 使用 Input.GetAxis 获取滚轮增量（如使用新输入系统请做相应修改）
    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
    
        // 累计滚轮输入量，方向也会保留（正值或负值）
        accumulatedScroll += scrollInput;
        accumulatedScroll = Mathf.Clamp(accumulatedScroll, (baseFOV - MaxFOV) / ZoomSpeed, (baseFOV - MinFOV) / ZoomSpeed);

        // 根据累计输入量计算目标 FOV
        float targetFOV = baseFOV - (accumulatedScroll * ZoomSpeed);
        targetFOV = Mathf.Clamp(targetFOV, MinFOV, MaxFOV);
        Debug.Log("Accumulated scroll: " + accumulatedScroll);
        Debug.Log("Target FOV: " + targetFOV);

        // 平滑过渡到目标 FOV
        CameraInCamera.fieldOfView = Mathf.Lerp(CameraInCamera.fieldOfView, targetFOV, Runner.DeltaTime * 10f);

        // 计算当前累计滚动量对应的步进（负值也会被计算进去）
        int currentStep = Mathf.FloorToInt(accumulatedScroll / scrollStep);
        // 当步进发生变化并且满足时间间隔后播放声音
        if (currentStep != lastZoomStep && Time.time - lastSoundTime >= minSoundInterval)
        {
            AudioManager.Instance.PlaySFX(this.gameObject, zoomSound, soundVolume);
            lastZoomStep = currentStep;
            lastSoundTime = Time.time;
        }

        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            if (CameraInCamera.fieldOfView >= MaxFOV * 0.95f)
            {
                if (!pendingSwitchState)
                    StartCoroutine(BeingPending());
                else
                {
                    _rigController.SwitchToHippie(3f);
                    _playerMovement.aimSpeed = _playerMovement.defaultSpeed;
                    pendingSwitchState = false;
                }
            }
            else
            {
                _rigController.SwitchToAim(3f);
                _playerMovement.aimSpeed = _originalAimSpeed;
            }
        }
}

IEnumerator BeingPending()
{
    yield return new WaitForSeconds(0.1f);
    pendingSwitchState = true;
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

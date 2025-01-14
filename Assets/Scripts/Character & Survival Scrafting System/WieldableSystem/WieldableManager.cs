using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

namespace LPSurvivalEngine
{
    public class WieldableManager : NetworkBehaviour
    {
        [Space]
        [Header("Wieldable Manager")]
        [Space]
        [Space]

        [HideInInspector] public Wieldable currentWieldable;
        public Transform wieldablesPosition;
        public Transform flashlightPosition;
        public Transform cameraPositon;
        public Transform AimPositon;
        public PlayerInput PlayerInput;
        private InputAction actionAction;

        public static WieldableManager instance;
        [HideInInspector] public PlayerController controller;


        private void Awake()
        {
            instance = this;
            // PlayerInput = GameObject.Find("InputManager").GetComponent<PlayerInput>();

            //if (PlayerInput != null) {
            //    actionAction = PlayerInput.actions.FindAction("Action");
            //    actionAction.performed += OnAttackInput;
            //}

            //Object.ReleaseStateAuthority();
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            Debug.Log("WieldAble��");
            if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
            {
                currentWieldable.OnAttackInput();
            }
        }

        public void OnAltAttackInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && currentWieldable != null && controller.cursor == true)
            {
                currentWieldable.OnAltAttackInput();
            }
        }

        public void EquipNewItem(ItemDatabase item)
        {
            // ��¼��ǰҪװ������Ʒ
            equippedItem = item;

            RequestStateAuthorityForEquipItem(Runner.LocalPlayer);

            // ���������������Ʒ
            RPC_RequestEquipItem(Runner.LocalPlayer);
        }

        // ��¼��ǰҪװ������Ʒ
        private ItemDatabase equippedItem;

        [Networked] public PlayerRef Owner { get; set; } // ����ͬ������Ʒ������

        // RPC ��������װ����Ʒ���ͻ��˵��ã�
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RequestEquipItem(PlayerRef player)
        {
            //// ֻ�� StateAuthority ����ִ�� Spawn
            if (Object.HasStateAuthority)
            {
            GameObject.Find("CurrentPlayer").GetComponent<FirstPersonOptimizer>().Wield();
            SpawnEquippedItem(player);
            }
        }

        private void RequestStateAuthorityForEquipItem(PlayerRef player)
        {
            // �����ǰ�ͻ���û�� StateAuthority����������
            if (!HasStateAuthority)
            {
                // �˴���α�ʾ�˶����ڵ�ǰ�ͻ�����û�п���Ȩ��
                Debug.Log("Requesting StateAuthority for EquipItem.");
                Object.RequestStateAuthority();
                if (HasStateAuthority)
                {
                    Debug.Log($"This client has StateAuthority over {gameObject.name}");
                }
                else
                {
                    Debug.Log($"This client does not have StateAuthority over {gameObject.name}");
                }
            }// �����ȡ�ö���Ŀ���Ȩ��
            else
            {
                Debug.Log("Already have StateAuthority.");
            }
        }

        public Transform CurrentWieldableRootTransform()
        {
            Transform spawnPosition = null;
            if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() == null)
            {
                GameObject currentPlayer = GameObject.Find("CurrentPlayer");
                spawnPosition = currentPlayer.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() != null)
            {
                spawnPosition = cameraPositon;
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() != null && equippedItem.wieldablePrefab.GetComponent<ConeDetection>() == null)
            {
                spawnPosition = flashlightPosition;
            }

            return spawnPosition;
        }

        // ��Ʒ�����߼���ֻ�� StateAuthority ִ�У�
        private void SpawnEquippedItem(PlayerRef player)
        {
            Owner = player;

/*            // ������Ʒ����ѡ������λ��
            Transform spawnPosition = null;
            if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<CameraController>() == null)
            {
                GameObject currentPlayer = GameObject.Find("CurrentPlayer");
                spawnPosition = currentPlayer.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() == null && equippedItem.wieldablePrefab.GetComponent<CameraController>() != null)
            {
                spawnPosition = cameraPositon;
            }
            else if (equippedItem.wieldablePrefab.GetComponent<Flashlight>() != null && equippedItem.wieldablePrefab.GetComponent<CameraController>() == null)
            {
                spawnPosition = flashlightPosition;
            }*/

            // ���û���ҵ�����λ�ã��׳�����
            if (CurrentWieldableRootTransform() == null)
            {
                Debug.LogError("Unexpected item type: " + equippedItem.wieldablePrefab.name);
                return;
            }

            // ʹ�� Runner.Spawn ʵ������ͬ����Ʒ
            NetworkObject spawnedItem = Runner.Spawn(equippedItem.wieldablePrefab, CurrentWieldableRootTransform().position, CurrentWieldableRootTransform().rotation);
            
            // ȷ�������ɵ���Ʒ���ص�������
            if (spawnedItem != null)
            {
                //// ������Ʒ�ĸ�����
                spawnedItem.transform.SetParent(CurrentWieldableRootTransform());
                spawnedItem.transform.localPosition = Vector3.zero;
                spawnedItem.transform.localRotation = Quaternion.identity;

                spawnedItem.RequestStateAuthority();

                // ����Ϊ��ǰװ����Ʒ
                if (spawnedItem.TryGetComponent<Wieldable>(out var wieldable))
                {
                    currentWieldable = wieldable; // ���µ�ǰװ������Ʒ
                    Debug.Log($"[SpawnEquippedItem] Equipped item: {equippedItem.wieldablePrefab.name} by {player}");

                    currentWieldable.player = player;
                }
            }
            else
            {
                Debug.LogError("Failed to spawn item: " + equippedItem.wieldablePrefab.name);
            }
        }

        public void DropWieldable()
        {
            if (currentWieldable != null)
            {
                Destroy(currentWieldable.gameObject);
                currentWieldable = null;
            }
        }
    }


    }
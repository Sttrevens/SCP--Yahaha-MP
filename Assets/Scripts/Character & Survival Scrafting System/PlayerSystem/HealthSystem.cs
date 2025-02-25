using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LPSurvivalEngine
{
    public class HealthSystem : NetworkBehaviour, IDamagable
    {
        //public static HealthSystem instance{get;private set;}

        [Header("Player Vitals")]

        public Vitals health;
        public Vitals stamina;
        public Vitals oxygen;
        public Vitals sanity;
        
        [Networked] public float playerHealth { get; set; } = 100f;
        [Networked] public float playerStamina { get; set; } = 100f;
        [Networked] public float playerOxygen { get; set; } = 100f;
        [Networked] public float playerSanity { get; set; } = 100f;

        [Header("Health System")]

        public UnityEvent onTakeDamage;

        [Header("Vitals Settings")]
    
        public float staminaHealthdecay;
        //氧气空的时候生命值下降的速度
        public float oxygenHealthdecay;

        public bool isInOxygenRoom;
        
        [Header("Effects")]
        
        public AudioClip scareSound;

        [Header("Assignments")]

        public GameObject Player;
        public GameObject UIPlayer;
        public GameObject sleepScreenAnimation;
        
        [Header("Post processing")]
        
        public CustomRendererFeature postProcessing;
        
        [Header("Player Effects")]
        
        public bool isScared = false;
        public bool isDying = false;
        public bool isTired = false;
        
        void Start()
        {
            // 保障措施
            health.currentValue = Mathf.Min(health.startValue, health.maxValue);
            stamina.currentValue = Mathf.Min(stamina.startValue, stamina.maxValue);
            oxygen.currentValue = Mathf.Min(oxygen.startValue, oxygen.maxValue);
            sanity.currentValue = Mathf.Min(sanity.startValue, sanity.maxValue);
            
            Player = gameObject;
            UIPlayer = GameObject.FindGameObjectWithTag("UI Player");

            Inventory.instance.vitals = this;

            if (UIPlayer != null)
            {
                sleepScreenAnimation = FindChildRecursive(UIPlayer.transform, "SleepAnimation");
                if (sleepScreenAnimation != null)
                {
                    // ����������ӻ�ȡ���������Ĳ���
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪSleepAnimation��������");
                }
                //获取生命值UI图像
                Image healthVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Health");
                if (healthVitalBarImage != null)
                {
                    health.VitalBar = healthVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪHealth��������");
                }
                //获取体力值UI图像
                Image staminaVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Stamina");
                if (staminaVitalBarImage != null)
                {
                    stamina.VitalBar = staminaVitalBarImage;
                }
                else
                {
                    Debug.Log("δҵΪStamina");
                }
                //获取氧气值UI
                Image oxygenVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Oxygen");
                if (oxygenVitalBarImage != null)
                {
                    oxygen.VitalBar = oxygenVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪOxygen");
                }
                //获取San值UI
                Image sanityVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Sanity");
                if (sanityVitalBarImage != null)
                {
                    sanity.VitalBar = sanityVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪSanity");
                }
            }
            else
            {
                Debug.Log("δ�ҵ�tagΪUI Player������");
            }
        }

        /*public override void Render()
        {
            postProcessing.enableMoHuPostProcessing = isTired;
            postProcessing.enableInvertColor = isScared;
        }*/
        
        private T FindChildRecursive<T>(Transform parent, string name) where T : Component
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
                T result = FindChildRecursive<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;  
        }

        private GameObject FindChildRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
                GameObject result = FindChildRecursive(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        void FixedUpdate()
        {
            if (gameObject.tag != "Player")
            {
                return;
            }
            if (!(Input.GetButton("Sprint") && GetComponent<PlayerMovement>().isMoving))
            {
                stamina.Add(stamina.regenrate * Time.fixedDeltaTime);
            }

            if (isInOxygenRoom)
            {
                oxygen.Add(oxygen.regenrate * Time.fixedDeltaTime);
            }
            else
            {
                oxygen.Subtract(oxygen.decayRate * Time.fixedDeltaTime);
            }
            sanity.Subtract(sanity.regenrate * Time.fixedDeltaTime);

            if (GetComponent<PlayerMovement>().issprinting)
            {
                stamina.Subtract(stamina.decayRate * Time.fixedDeltaTime);
            }

            //生存版本的血量恢复, 暂时禁用
            // if ((stamina.currentValue >= stamina.maxValue * 0.8f) && (oxygen.currentValue >= oxygen.maxValue * 0.8f) && (sanity.currentValue >= sanity.maxValue * 0.5f))
            //     health.Add(health.regenrate * Time.fixedDeltaTime);

            if (oxygen.currentValue == 0.0f)
            {
                health.Subtract(oxygenHealthdecay * Time.fixedDeltaTime);
            }
        
            if (health.currentValue == 0.0f && gameObject.CompareTag("Player"))
            {
                Rpc_Die();
            }

            if (HasStateAuthority)
            {
                UpdateUIAndSync();
                HandlePlayerEffects();
            }
        }
        

        void HandlePlayerEffects()
        {
            if (playerHealth <= 30)
            {
                isDying = true;
                
            }
            else
            {
                isDying = false;
            }
            
            if (playerSanity <= 20)
            {
                isScared = true;
                Inventory.instance.DropItem();
                postProcessing.enableInvertColor = true;
            }
            else
            {
                isScared = false;
                postProcessing.enableInvertColor = false;
            }
            
            if (playerStamina <= 10)
            {
                isTired = true;
                postProcessing.enableMoHuPostProcessing = true;
            }
            else
            {
                isTired = false;
                postProcessing.enableMoHuPostProcessing = false;
            }
        }

        private void UpdateUIAndSync()
        {
            health.VitalBar.fillAmount = health.GetPercentage();
            stamina.VitalBar.fillAmount = stamina.GetPercentage();
            oxygen.VitalBar.fillAmount = oxygen.GetPercentage();
            sanity.VitalBar.fillAmount = sanity.GetPercentage();

            if (playerHealth != health.currentValue)
            {
                SynchronousPlayerHealthRpc();
            }
            if (playerStamina != stamina.currentValue)
            {
                SynchronousPlayerStaminaRpc();
            }
            if (playerOxygen != oxygen.currentValue)
            {
                SynchronousPlayerOxygenRpc();
            }
            if (playerSanity != sanity.currentValue)
            {
                SynchronousPlayerSanityRpc();
            }

            // if (isScared)
            // {
            //     GameObject.FindObjectOfType<Test>().GetComponent<Test>().Mohu();
            //     Inventory.instance.DropItem();
            // }
            // else
            // {
            //     GameObject.FindObjectOfType<Test>().GetComponent<Test>().NoMohu();
            // }
        }

        public void Heal(float amount)
        {
            health.Add(amount);
        }

        public void Eat(float amount)
        {
            stamina.Add(amount);
        }

        public void Drink(float amount)
        {
            oxygen.Add(amount);
        }

        public void Sleep(float amount)
        {
            sanity.Add(amount);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_TakePhysicDamage(int amount)
        {
            TakePhysicDamage(amount);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_Scared(float amount)
        {
            Scared(amount);
        }

        public void TakePhysicDamage(int amount)
        {
            health.Subtract(amount);
            onTakeDamage?.Invoke();
            GameObject parentObject = GameObject.FindGameObjectWithTag("UI Player");
            if (parentObject != null)
            {
                DamageIndicator bloodScreen = FindObjectOfType<DamageIndicator>();
                if (bloodScreen != null)
                {
                    bloodScreen.Flash();
                }
                else
                {
                    Debug.Log("δ�ҵ���Ϊ��������");
                }
            }
            else
            {
                Debug.Log("δ�ҵ�tagΪ ������");
            }
        }
        
        public void Scared(float amount)
        {
            sanity.Subtract(amount);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Rpc_Die()
        {
            Die();
        }
        
        private ScreenFade screenFade;
        private GameObject viewerBobojian;
    
        public void Die()
        {
            //Player.SetActive(false);
            //UIPlayer.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GetComponent<Rigidbody>().isKinematic = false;
            //Inventory.instance.inventoryWindow.SetActive(true);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponent<Rigidbody>().useGravity = true;
            Camera.main.GetComponent<FirstPersonCamera>().isCameraLocked = true;
            GetComponent<NavMeshObstacle>().enabled = false;
            gameObject.tag = "Untagged";
            foreach (var item in Inventory.instance.slots)
            {
                if (item != null)
                {
                    Inventory.instance.selectedItem = item;
                    Inventory.instance.DropItem();
                }
            }
            
            screenFade = FindObjectOfType<ScreenFade>();
            StartCoroutine(Dying());
        }

        private IEnumerator Dying()
        {
            ControlSticksController.Instance.HandlePlayerDeath();
            screenFade.TriggerScreenFadeOnly();
            viewerBobojian = GameObject.Find("BobojianSystem").transform.Find("ViewerBobojian").gameObject;
            yield return new WaitForSeconds(screenFade.fadeDuration);
            viewerBobojian.SetActive(true);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void Rpc_Respawn()
        {
            StopAllCoroutines();
            
            //if (gameObject.tag == "Untagged")
            StartCoroutine(Respawning());
        }

        private IEnumerator Respawning()
        {
            screenFade.TriggerScreenFadeOnly();
            yield return new WaitForSeconds(screenFade.fadeDuration);
            Respawn();
        }

        public void Respawn()
        {
            viewerBobojian.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
Camera.main.GetComponent<FirstPersonCamera>().isCameraLocked = false;
GetComponent<NavMeshObstacle>().enabled = true;
gameObject.tag = "Player";
            health.currentValue = health.maxValue;
            stamina.currentValue = stamina.maxValue;
            sanity.currentValue = sanity.maxValue;
            GetComponent<NetworkTransform>().Teleport(Vector3.zero, Quaternion.identity);
        }

        #region һ��ͬ����ɫ���ݺ���

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SynchronousPlayerHealthRpc()
        {
            playerHealth = health.currentValue;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SynchronousPlayerStaminaRpc()
        {
            playerStamina = stamina.currentValue;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SynchronousPlayerOxygenRpc()
        {
            playerOxygen = oxygen.currentValue;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SynchronousPlayerSanityRpc()
        {
            playerSanity = sanity.currentValue;
        }

        #endregion
    }

    [System.Serializable]
    public class Vitals
    {
        [HideInInspector]
        public float currentValue;
        public float maxValue;
        public float startValue;
        public float regenrate;
        public float decayRate;

        public Image VitalBar;

        public void Add(float amount)
        {
            currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        }

        public void Subtract(float amount)
        {
            currentValue = Mathf.Clamp(currentValue - amount, 0, maxValue);
        }
    
        public float GetPercentage()
        {
            return currentValue / maxValue;
        }
    }

    public interface IDamagable
    {
        void TakePhysicDamage(int damageAmount);
    }
}
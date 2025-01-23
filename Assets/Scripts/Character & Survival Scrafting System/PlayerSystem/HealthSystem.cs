using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
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
        public float oxygenHealthdecay;

        [Header("Assignments")]

        public GameObject Player;
        public GameObject UIPlayer;
        public GameObject sleepScreenAnimation;

        void Start()
        {
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

                Image healthVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Health");
                if (healthVitalBarImage != null)
                {
                    health.VitalBar = healthVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪHealth��������");
                }

                Image staminaVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Stamina");
                if (staminaVitalBarImage != null)
                {
                    stamina.VitalBar = staminaVitalBarImage;
                }
                else
                {
                    Debug.Log("δҵΪStamina");
                }

                Image oxygenVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Oxygen");
                if (oxygenVitalBarImage != null)
                {
                    oxygen.VitalBar = oxygenVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪOxygen");
                }

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
            // 使用 Time.fixedDeltaTime 替代 Time.deltaTime
            stamina.Add(stamina.regenrate * Time.fixedDeltaTime);
            oxygen.Subtract(oxygen.decayRate * Time.fixedDeltaTime);
            sanity.Subtract(sanity.regenrate * Time.fixedDeltaTime);

            if ((stamina.currentValue >= stamina.maxValue * 0.8f) && (oxygen.currentValue >= oxygen.maxValue * 0.8f) && (sanity.currentValue >= sanity.maxValue * 0.5f))
                health.Add(health.regenrate * Time.fixedDeltaTime);

            if (oxygen.currentValue == 0.0f)
            {
                health.Subtract(oxygenHealthdecay * Time.fixedDeltaTime);
            }
        
            if (health.currentValue == 0.0f)
            {
                Die();
            }

            UpdateUIAndSync();
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

        public void TakePhysicDamage(int amount)
        {
            health.Subtract(amount);
            onTakeDamage?.Invoke();
            GameObject parentObject = GameObject.FindGameObjectWithTag("UI Player");
            if (parentObject != null)
            {
                GameObject bloodScreen = FindChildRecursive(parentObject.transform, "BloodScreen");
                if (bloodScreen != null)
                {
                    bloodScreen.GetComponent<DamageIndicator>().Flash();
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
    
        public void Die()
        {
            //Player.SetActive(false);
            //UIPlayer.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GetComponent<Rigidbody>().isKinematic = false;
            //Inventory.instance.inventoryWindow.SetActive(true);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            gameObject.tag = "Untagged";
            // foreach (var item in Inventory.instance.slots)
            // {
            //     Inventory.instance.ThrowItem(item.item);
            //     item.quantity--;
            // }
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
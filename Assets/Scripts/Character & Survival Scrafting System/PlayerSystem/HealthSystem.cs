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
        public Vitals hunger;
        public Vitals thirst;
        public Vitals sanity;
        
        [Networked] public float playerHealth { get; set; } = 100f;
        [Networked] public float playerHunger { get; set; } = 100f;
        [Networked] public float playerThirst { get; set; } = 100f;
        [Networked] public float playerSanity { get; set; } = 100f;

        [Header("Health System")]

        public UnityEvent onTakeDamage;

        [Header("Vitals Settings")]
    
        public float hungerHealthdecay;
        public float thirstHealthdecay;

        [Header("Assignments")]

        public GameObject Player;
        public GameObject UIPlayer;
        public GameObject sleepScreenAnimation;

        void Start()
        {
    health.currentValue = Mathf.Min(health.startValue, health.maxValue);
    hunger.currentValue = Mathf.Min(hunger.startValue, hunger.maxValue);
    thirst.currentValue = Mathf.Min(thirst.startValue, thirst.maxValue);
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

                Image hungerVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Hunger");
                if (hungerVitalBarImage != null)
                {
                    hunger.VitalBar = hungerVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪHunger��������");
                }

                Image thirstVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Thirst");
                if (thirstVitalBarImage != null)
                {
                    thirst.VitalBar = thirstVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪThirst��������");
                }

                Image sanityVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Sleep");
                if (sanityVitalBarImage != null)
                {
                    sanity.VitalBar = sanityVitalBarImage;
                }
                else
                {
                    Debug.Log("δ�ҵ���ΪSleep��������");
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
            hunger.Subtract(hunger.decayRate * Time.fixedDeltaTime);
            thirst.Subtract(thirst.decayRate * Time.fixedDeltaTime);
            sanity.Subtract(sanity.regenrate * Time.fixedDeltaTime);

            if ((hunger.currentValue >= hunger.maxValue * 0.8f) && (thirst.currentValue >= thirst.maxValue * 0.8f) && (sanity.currentValue >= sanity.maxValue * 0.5f))
                health.Add(health.regenrate * Time.fixedDeltaTime);

            if (hunger.currentValue == 0.0f)
            {
                health.Subtract(hungerHealthdecay * Time.fixedDeltaTime);
            }

            if (thirst.currentValue == 0.0f)
            {
                health.Subtract(thirstHealthdecay * Time.fixedDeltaTime);
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
            hunger.VitalBar.fillAmount = hunger.GetPercentage();
            thirst.VitalBar.fillAmount = thirst.GetPercentage();
            sanity.VitalBar.fillAmount = sanity.GetPercentage();

            if (playerHealth != health.currentValue)
            {
                SynchronousPlayerHealthRpc();
            }
            if (playerHunger != hunger.currentValue)
            {
                SynchronousPlayerHungerRpc();
            }
            if (playerThirst != thirst.currentValue)
            {
                SynchronousPlayerThirstRpc();
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
            hunger.Add(amount);
        }

        public void Drink(float amount)
        {
            thirst.Add(amount);
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
        public void SynchronousPlayerHungerRpc()
        {
            playerHunger = hunger.currentValue;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SynchronousPlayerThirstRpc()
        {
            playerThirst = thirst.currentValue;
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
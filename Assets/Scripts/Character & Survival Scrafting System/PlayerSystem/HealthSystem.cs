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
        public Vitals sleep;
        
        [Networked] public float playerHealth { get; set; } = 100f;
        [Networked] public float playerHunger { get; set; } = 100f;
        [Networked] public float playerThirst { get; set; } = 100f;
        [Networked] public float playerSleep { get; set; } = 100f;

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
            health.currentValue = health.startValue;
            hunger.currentValue = hunger.startValue;
            thirst.currentValue = thirst.startValue;
            sleep.currentValue = sleep.startValue;

            Player = gameObject;
            UIPlayer = GameObject.FindGameObjectWithTag("UI Player");

            Inventory.instance.vitals = this;

            if (UIPlayer != null)
            {
                sleepScreenAnimation = FindChildRecursive(UIPlayer.transform, "SleepAnimation");
                if (sleepScreenAnimation != null)
                {
                    // 这里可以添加获取到子物体后的操作
                }
                else
                {
                    Debug.Log("未找到名为SleepAnimation的子物体");
                }

                Image healthVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Health");
                if (healthVitalBarImage != null)
                {
                    health.VitalBar = healthVitalBarImage;
                }
                else
                {
                    Debug.Log("未找到名为Health的子物体");
                }

                Image hungerVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Hunger");
                if (hungerVitalBarImage != null)
                {
                    hunger.VitalBar = hungerVitalBarImage;
                }
                else
                {
                    Debug.Log("未找到名为Hunger的子物体");
                }

                Image thirstVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Thirst");
                if (thirstVitalBarImage != null)
                {
                    thirst.VitalBar = thirstVitalBarImage;
                }
                else
                {
                    Debug.Log("未找到名为Thirst的子物体");
                }

                Image sleepVitalBarImage = FindChildRecursive<Image>(UIPlayer.transform, "Sleep");
                if (sleepVitalBarImage != null)
                {
                    sleep.VitalBar = sleepVitalBarImage;
                }
                else
                {
                    Debug.Log("未找到名为Sleep的子物体");
                }
            }
            else
            {
                Debug.Log("未找到tag为UI Player的物体");
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

        void Update()
        {
            hunger.Subtrack(hunger.decayRate * Time.deltaTime);
            thirst.Subtrack(thirst.decayRate * Time.deltaTime);
            sleep.Subtrack(sleep.regenrate * Time.deltaTime);

            if ((hunger.currentValue >= hunger.maxValue * 0.8f) && (thirst.currentValue >= thirst.maxValue * 0.8f) && (sleep.currentValue >= sleep.maxValue * 0.5f))
                health.Add(health.regenrate * Time.deltaTime);

            if (hunger.currentValue == 0.0f)
            {
                health.Subtrack(hungerHealthdecay * Time.deltaTime);
            }

            if (thirst.currentValue == 0.0f)
            {
                health.Subtrack(thirstHealthdecay * Time.deltaTime);
            }
        
            if (health.currentValue == 0.0f)
            {
                Die();
            }

            health.VitalBar.fillAmount = health.GetPercentage();
            hunger.VitalBar.fillAmount = hunger.GetPercentage();
            thirst.VitalBar.fillAmount = thirst.GetPercentage();
            sleep.VitalBar.fillAmount = sleep.GetPercentage();
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
            if (playerSleep != sleep.currentValue)
            {
                SynchronousPlayerSleepRpc();
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
            sleep.Add(amount);
        }

        public void TakePhysicDamage(int amount)
        {
            health.Subtrack(amount);
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
                    Debug.Log("未找到名为的子物体");
                }
            }
            else
            {
                Debug.Log("未找到tag为 的物体");
            }
        }
    
        public void Die()
        {
            //Player.SetActive(false);
            //UIPlayer.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Respawn");
        }

        #region 一凡同步角色数据函数

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
        public void SynchronousPlayerSleepRpc()
        {
            playerSleep = sleep.currentValue;
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
            currentValue = Mathf.Min(currentValue + amount, maxValue);
        }

        public void Subtrack(float amount)
        {
            currentValue = Mathf.Max(currentValue - amount, 0);
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
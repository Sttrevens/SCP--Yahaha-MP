using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LPSurvivalEngine
{
    public class HealthSystem : MonoBehaviour, IDamagable
    {
    public static HealthSystem instance{get;private set;}

    [Header("Player Vitals")]

    public Vitals health;
    public Vitals hunger;
    public Vitals thirst;
    public Vitals sleep;

    [Header("Health System")]

    public UnityEvent onTakeDamage;

    [Header("Vitals Settings")]
    
    public float hungerHealthdecay;
    public float thirstHealthdecay;

    [Header("Assignments")]

    public GameObject Player;
    public GameObject UIPlayer;



    private void Awake()
    {
        if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject); 
            }
    }

    void Start()
    {
        health.currentValue = health.startValue;
        hunger.currentValue = hunger.startValue;
        thirst.currentValue = thirst.startValue;
        sleep.currentValue = sleep.startValue;
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
        sleep.Subtrack(amount);
    }

    public void TakePhysicDamage(int amount)
    {
        health.Subtrack(amount);
        onTakeDamage?.Invoke();
    }
    
    public void Die()
    {
        //Player.SetActive(false);
        //UIPlayer.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Respawn");
    }
     
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
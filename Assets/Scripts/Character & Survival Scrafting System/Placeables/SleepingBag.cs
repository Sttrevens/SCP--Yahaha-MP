using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace LPSurvivalEngine
{
    public class SleepingBag : Buildings, IInteractable
    {
    [Space]
    [Header("Sleeping Bag")]
    [Space]

    [Space]
    [Header("Sleep Time")]
    [Space]

    public float wakupTime;
    public float startSleepTime;
    public float endSleepTime;

    [Space]
    [Header("Sleep")]

    public float sleepVital;

    [Space]
    [Header("Assignments")]
    [Space]

    public RawImage SleepScreen;
    public GameObject Message;
    public TextMeshProUGUI MessageText;

    private Animator anim;
    private InputManager inputManager;

    
    void Awake()
    {
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    public string GetInteractText()
    {
        return CanSleep() ? "Sleep" : "Sleep at night";
    }
    
    private void Start()
    {
        SleepScreen = FindObjectOfType<RawImage>(true);
        Message = GameObject.FindGameObjectWithTag("Message");
        //MessageText = Message.GetComponent<TextMeshProUGUI>();
    }

    public void OnInteract()    
    {
        if (CanSleep())
        {
            anim.SetTrigger("Pickup");

            SleepScreen.GetComponent<Animation>().Play("sleep");
            
            TimeSystem.instance.time = wakupTime;
           
            HealthSystem.instance.Sleep(sleepVital);
        }

        if(!CanSleep())
        {
            //MessageText.text = "You can only sleep at night";
            //Message.SetActive(true);
            //StartCoroutine(MessageTime());
        }
    }

    /*IEnumerator MessageTime()
    {
        yield return new WaitForSeconds(2);
        Message.SetActive(false);
    }*/

    bool CanSleep()
    {
        return TimeSystem.instance.time >= startSleepTime || TimeSystem.instance.time < endSleepTime;
    }
    
}
    
}
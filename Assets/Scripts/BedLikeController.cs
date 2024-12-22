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
    public class BedLikeController : Buildings, IInteractable
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

        private GameObject player;

    public string GetInteractText()
    {
        return CanSleep() ? "Sleep" : "Sleep at night";
    }
    
    private void Start()
    {
        SleepScreen = player.GetComponentInChildren<RawImage>(true);
            //Message = GameObject.FindGameObjectWithTag("Message");
        //MessageText = Message.GetComponent<TextMeshProUGUI>();
    }

    public void OnInteract()    
    {
            anim = player.GetComponent<Animator>();

            anim.SetTrigger("Pickup");

            SleepScreen.GetComponent<Animation>().Play("sleep");

            player.GetComponent<HealthSystem>().Sleep(sleepVital);
            
            //TimeSystem.instance.time = wakupTime;
          
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

        public void SetPlayer(GameObject player)
        {
            this.player = player;
        }

    }
    
}
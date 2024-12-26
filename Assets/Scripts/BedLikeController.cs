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

    public GameObject SleepScreen;
    public GameObject Message;
    public TextMeshProUGUI MessageText;
        public GameObject minigamepanel;
        
    private Animator anim;
    private InputManager inputManager;
    private MinigameController minigameController;
    private GameObject player;


    public string GetInteractText()
    {
        return CanSleep() ? "Sleep" : "Sleep at night";
    }
    
    private void Start()
    {
            //Message = GameObject.FindGameObjectWithTag("Message");
            //MessageText = Message.GetComponent<TextMeshProUGUI>();
            minigameController = minigamepanel.GetComponent<MinigameController>();
            minigamepanel.SetActive(false);
            minigameController = minigamepanel.GetComponent<MinigameController>();
            minigameController.onMinigameEnd += OnMinigameEnd;
            minigamepanel.SetActive(false);

        }

    public void OnInteract()    
    {
            anim = player.GetComponent<Animator>();

            anim.SetTrigger("Pickup");

            SleepScreen.GetComponent<Animation>().Play("sleep");

            player.GetComponent<HealthSystem>().Sleep(sleepVital);

            
            minigamepanel.SetActive(true);

           
            if (minigameController != null)
            {
                minigameController.StartMinigame();
            }

    }

        
    public void OnMinigameEnd(int finalScore)
    {
          
            Debug.Log("小游戏结束，得分 = " + finalScore);

            
            minigamepanel.SetActive(false);

           
           
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
            SleepScreen = player.GetComponent<HealthSystem>().sleepScreenAnimation;
        }

    }
    
}
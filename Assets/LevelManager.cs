using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Globalization;
using LPSurvivalEngine;

public class LevelManager : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject uiPanel;
    private bool isPaused = false;
    [SerializeField] private bool IsStarted = false;
    
    [SerializeField]private Vector3[] LevelOffsets;
    [SerializeField]private GameObject[] Levels;

    private void Start()
    {
        Time.timeScale = 1;
    }

    public string GetInteractText()
    {
        return string.Format("{0}", IsStarted ? "No Way Back" : "Select level");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_OnInteract()
    {
        PauseGame();
    }
    
    public void OnInteract()
    {
        Rpc_OnInteract(); 
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0;  
        uiPanel.SetActive(true); 
        isPaused = true;
        PlayerController.instance.ToggleCursor(true);
    }

 
    public void ResumeGame()
    {
        Time.timeScale = 1;  
        uiPanel.SetActive(false);  
        isPaused = false;
        PlayerController.instance.ToggleCursor(false);
    }

    public void LoadLevel(int index)
    {
        int levelNumber = index - 1;
        GameObject level = Levels[levelNumber];
        Vector3 offset = LevelOffsets[levelNumber];
        Instantiate(level,offset, Quaternion.identity);
    }


    
    
    
}

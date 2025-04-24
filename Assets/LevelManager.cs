
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Globalization;
using LPSurvivalEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class LevelManager : NetworkBehaviour, IInteractable
{
    public static LevelManager Instance { get; private set; } 

    // 添加关卡加载和销毁的事件
    public event Action OnLevelLoaded;
    public event Action OnLevelDestroyed;

    [SerializeField] private GameObject uiPanel;
    private bool isPaused = false;
    [Networked] public bool IsStarted { get; set; } = false;

    [SerializeField] private Vector3[] LevelOffsets;
    [SerializeField] private GameObject[] Levels;
    //selection btn control
    [Networked] public bool isButtonSelected { get; set; } = false;
    [Networked] public int roomIndexSelected { get; set; } = -1;
    private NetworkObject currentLevel;
    private bool isLevelSelectionDisabled;

     public NavMeshSurface surface;
     
     public Light directionalLight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        isButtonSelected = false;
        roomIndexSelected = -1;
        isLevelSelectionDisabled = false;
    }

    public string GetInteractText()
    {
        return string.Format("{0}", isLevelSelectionDisabled ? "level selection no longer available" : "Select level");
    }

    public void OnInteract()
    {
        if(!isLevelSelectionDisabled) PauseGame();
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_LoadLevel()
    {
        isLevelSelectionDisabled = true;
        int levelNumber = roomIndexSelected;
        GameObject level = Levels[levelNumber];
        Vector3 offset = LevelOffsets[levelNumber];
        currentLevel = Runner.Spawn(level, offset, Quaternion.identity);
        RPC_BuildNavMesh();
        
        // 触发关卡加载完成事件
        RPC_LevelLoaded();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_LevelLoaded()
    {
        // 触发关卡加载完成事件
        OnLevelLoaded?.Invoke();
        UnityEngine.Debug.Log("关卡加载完成事件已触发");
    }

    public void LoadLevel()
    {
        Rpc_UpdateDirectionalLight(0.1f);
        RPC_LoadLevel();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_UpdateDirectionalLight(float intensity)
    {
        directionalLight.intensity = intensity;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DestroyLevel()
    {
        isLevelSelectionDisabled = false;
        if(currentLevel != null)
        {
            Runner.Despawn(currentLevel);
        }

        foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            if (!enemy.GetComponent<EyedSlimeBlueController>())
                Destroy(enemy.gameObject);
        }
        
        // 触发关卡销毁事件
        RPC_LevelDestroyed();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_LevelDestroyed()
    {
        // 触发关卡销毁事件
        OnLevelDestroyed?.Invoke();
        UnityEngine.Debug.Log("关卡销毁事件已触发");
    }

    public void DestroyLevel()
    {
        Rpc_UpdateDirectionalLight(1.5f);
        RPC_DestroyLevel();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateAllButtons()
    {
        SelectionButton[] buttons = FindObjectsOfType<SelectionButton>();
        foreach (var button in buttons)
        {
            button.UpdateButtonAppearance();
        }
    }

    public void PlayButtonSound()
    {
        AudioManager.Instance.PlayStartButtonSound();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BuildNavMesh()
    {
        LevelAIManager levelAIManager = currentLevel.GetComponent<LevelAIManager>();
        if (levelAIManager != null)
        {
            if (surface != null)
            {
                surface.navMeshData = levelAIManager.navMeshData;
            }

            if (levelAIManager.onLevelLoaded != null)
                levelAIManager.onLevelLoaded.Invoke();
        }
    }
}
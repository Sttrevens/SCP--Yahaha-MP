using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Globalization;
using LPSurvivalEngine;
using Unity.AI.Navigation;

public class LevelManager : NetworkBehaviour, IInteractable
{
    public static LevelManager Instance { get; private set; } 

    [SerializeField] private GameObject uiPanel;
    private bool isPaused = false;
    [SerializeField] private bool IsStarted = false;

    [SerializeField] private Vector3[] LevelOffsets;
    [SerializeField] private GameObject[] Levels;
    //selection btn control
    [HideInInspector] public bool isButtonSelected;
    [HideInInspector] public int roomIndexSelected;
    private NetworkObject currentLevel;
    private bool isLevelSelectionDisabled;

     public NavMeshSurface surface;

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
    }

    public void LoadLevel()
    {
        RPC_LoadLevel();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DestroyLevel()
    {
        isLevelSelectionDisabled = false;
        if(currentLevel != null)
        {
            Runner.Despawn(currentLevel);
        }
        RPC_BuildNavMesh();
    }

    public void DestroyLevel()
    {
        RPC_DestroyLevel();
    }
    
    public void UpdateAllButtons()
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
        StartCoroutine(BuildNavMeshAsync());
    }

    IEnumerator BuildNavMeshAsync()
    {
        if (surface != null)
        {
            surface.BuildNavMesh();
            yield return null;
        }
    }
}

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

    public void LoadLevel()
    {
        isLevelSelectionDisabled = true;
        int levelNumber = roomIndexSelected;
        GameObject level = Levels[levelNumber];
        Vector3 offset = LevelOffsets[levelNumber];
        currentLevel = Runner.Spawn(level, offset, Quaternion.identity);
        StartCoroutine(BuildNavMeshAsync());
    }

    public void DestroyLevel()
    {
        isLevelSelectionDisabled = false;
        Destroy(currentLevel.gameObject);
        StartCoroutine(BuildNavMeshAsync());
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

    IEnumerator BuildNavMeshAsync()
    {
        // 异步构建 NavMesh
        if (surface != null)
        {
            // 在每个更新循环中分步构建 NavMesh
            surface.BuildNavMesh();

            // 等待下一个帧
            yield return null;
        }
    }
}

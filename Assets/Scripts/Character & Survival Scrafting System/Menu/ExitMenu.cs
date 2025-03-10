using LPSurvivalEngine;
using System.Xml.Serialization;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    public static ExitMenu instance{get;private set;}

    public GameObject exitMenuPanel; // 关联退出菜单的 Panel

    [Header("Assignments")]
    public GameObject exitConfirm;
    public GameObject allButtons;

    [HideInInspector] public bool isPaused = false;

    public string titleMenuSceneName;

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

    private void Start()
    {
        exitConfirm.SetActive(false);
    }


    // 显示退出菜单
    public void ShowExitMenu()
    {
        exitMenuPanel.SetActive(true); 
        if (GameObject.Find("CurrentPlayer").CompareTag("Player"))
            PlayerController.instance.ToggleCursor(true);
        //Time.timeScale = 0; 

        isPaused = true;
    }

    // 继续游戏
    public void HideExitMenu()
    {
        if (GameObject.Find("CurrentPlayer").CompareTag("Player"))
            PlayerController.instance.ToggleCursor(false);
        exitMenuPanel.SetActive(false);
        
        //Time.timeScale = 1; 

        isPaused = false;
    }


    // 返回主菜单
    public void ExitGame()
    {
        exitConfirm.SetActive(true);
        allButtons.SetActive(false);
    }

    public void RegretExitGame()
    {
        allButtons.SetActive(true);
        exitConfirm.SetActive(false);
    }

    public void ConfirmExitGame()
    {
        TitleScreenUI.roomName = "";
TitleScreenUI.playerName = "";
TitleScreenUI.Region = "";
TitleScreenUI.IsSpGame = false;
TitleScreenUI.IsSpectator = false;
NetworkSceneManagerDefault sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
sceneManager.Initialize(FindFirstObjectByType<NetworkRunner>());
FindFirstObjectByType<NetworkRunner>().Shutdown(destroyGameObject: true, 
    shutdownReason: ShutdownReason.Ok);
SceneManager.LoadScene(titleMenuSceneName);
    }
}
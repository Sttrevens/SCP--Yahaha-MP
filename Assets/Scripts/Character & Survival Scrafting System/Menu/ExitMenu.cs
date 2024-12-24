using LPSurvivalEngine;
using System.Xml.Serialization;
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

    public PlayerController playerController;

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
        playerController.ToggleCursor(true);
        //Time.timeScale = 0; 
    }

    // 继续游戏
    public void HideExitMenu()
    {
        exitMenuPanel.SetActive(false);
        playerController.ToggleCursor(false);
        //Time.timeScale = 1; 
    }


    // 返回主菜单
    public void ExitGame()
    {
        exitConfirm.SetActive(true);
        allButtons.SetActive(false);
    }

    public void RegretExitGame()
    {
        allButtons.SetActive(true) ;
        exitConfirm.SetActive(false);
    }

    public void ConfirmExitGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
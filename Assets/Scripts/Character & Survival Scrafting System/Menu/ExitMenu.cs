using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    public static ExitMenu instance{get;private set;}

    public GameObject exitMenuPanel; // 关联退出菜单的 Panel

    [Header("Button")]
    public Button continueButton;
    public Button exitButton;


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
        // 动态绑定按钮事件
        continueButton.onClick.AddListener(() => HideExitMenu());
        exitButton.onClick.AddListener(() => ExitGame());
    }


    // 显示退出菜单
    public void ShowExitMenu()
    {
        exitMenuPanel.SetActive(true); 
        Time.timeScale = 0; 
    }

    // 继续游戏
    public void HideExitMenu()
    {
        exitMenuPanel.SetActive(false); 
        Time.timeScale = 1; 
    }


    // 返回主菜单
    public void ExitGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
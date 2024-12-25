using TMPro;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private TextMeshProUGUI tmpText; // 显示文字的 TMP 对象
    [SerializeField] private TextMeshProUGUI roomNameText; // 显示房间名字的 TMP 对象
    [SerializeField] private TMP_InputField inputFieldTMP;

    // 各按钮对应的文字
    private const string creatingGameText = "Creating a Game...";
    private const string joiningGameText = "Joining a Game...";
    private const string destinationText = "Going to the Destination...";
    public static string roomName;

    // 初始化
    void Start()
    {
        roomNameText.text = roomName;
        tmpText.gameObject.SetActive(false); // 默认隐藏 TMP 对象
    }

    /// <summary>
    /// 按下“创建房间”按钮时调用
    /// </summary>
    public void OnCreateRoomButton()
    {
        HandleButtonClick(creatingGameText);
    }

    /// <summary>
    /// 按下“加入房间”按钮时调用
    /// </summary>
    public void OnJoinRoomButton()
    {
        HandleButtonClick(joiningGameText);
    }

    /// <summary>
    /// 按下“制作人名单”按钮时调用
    /// </summary>
    public void OnDestinationButton()
    {
        HandleButtonClick(destinationText);
    }

    /// <summary>
    /// 处理按钮点击逻辑
    /// </summary>
    /// <param name="displayText">需要显示的文字</param>
    public void HandleButtonClick(string displayText)
    {
        HideAllButtons(); // 隐藏所有按钮
        ShowText(displayText); // 显示对应文字
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            roomName = inputFieldTMP.text;
            SceneManager.LoadScene("[YiFan]PreDesignedLevel2");
        }
    }

    /// <summary>
    /// 隐藏所有按钮
    /// </summary>
    private void HideAllButtons()
    {
        buttonsParent.SetActive(false);
    }

    /// <summary>
    /// 显示 TMP 文字
    /// </summary>
    /// <param name="displayText">需要显示的文字</param>
    private void ShowText(string displayText)
    {
        tmpText.text = displayText; // 设置 TMP 文字
        tmpText.gameObject.SetActive(true); // 显示 TMP 对象
    }

    public void ResetUI()
    {
        tmpText.gameObject.SetActive(false);
        buttonsParent.SetActive(true);
    }
}
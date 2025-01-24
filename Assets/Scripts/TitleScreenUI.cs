using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private TextMeshProUGUI tmpText; // ��ʾ���ֵ� TMP ����
    [SerializeField] private TextMeshProUGUI roomNameText; // ��ʾ�������ֵ� TMP ����
    [SerializeField] private TMP_InputField inputFieldTMP;
    [SerializeField] private TMP_InputField playerNameInputField;

    // ����ť��Ӧ������
    private const string creatingGameText = "Creating a Game...";
    private const string joiningGameText = "Joining a Game...";
    private const string destinationText = "Going to the Destination...";
    public static string roomName;
    public static string playerName;

    public string gameSceneName;

    // ��ʼ��
    void Start()
    {
        roomNameText.text = roomName;
        tmpText.gameObject.SetActive(false); // Ĭ������ TMP ����
    }

    /// <summary>
    /// ���¡��������䡱��ťʱ����
    /// </summary>
    public void OnCreateRoomButton()
    {
        HandleButtonClick(creatingGameText);
    }

    /// <summary>
    /// ���¡����뷿�䡱��ťʱ����
    /// </summary>
    public void OnJoinRoomButton()
    {
        HandleButtonClick(joiningGameText);
    }

    /// <summary>
    /// ���¡���������������ťʱ����
    /// </summary>
    public void OnDestinationButton()
    {
        HandleButtonClick(destinationText);
    }

    /// <summary>
    /// ������ť����߼�
    /// </summary>
    /// <param name="displayText">��Ҫ��ʾ������</param>
    public void HandleButtonClick(string displayText)
    {
        HideAllButtons(); // �������а�ť
        ShowText(displayText); // ��ʾ��Ӧ����

            roomName = inputFieldTMP.text;
            playerName = playerNameInputField.text;
            SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// �������а�ť
    /// </summary>
    private void HideAllButtons()
    {
        buttonsParent.SetActive(false);
    }

    /// <summary>
    /// ��ʾ TMP ����
    /// </summary>
    /// <param name="displayText">��Ҫ��ʾ������</param>
    private void ShowText(string displayText)
    {
        tmpText.text = displayText; // ���� TMP ����
        tmpText.gameObject.SetActive(true); // ��ʾ TMP ����
    }

    public void ResetUI()
    {
        tmpText.gameObject.SetActive(false);
        buttonsParent.SetActive(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
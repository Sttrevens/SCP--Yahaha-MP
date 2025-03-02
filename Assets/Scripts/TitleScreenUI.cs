using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Linq;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private TextMeshProUGUI tmpText; // ��ʾ���ֵ� TMP ����
    [SerializeField] private TextMeshProUGUI roomNameText; // ��ʾ�������ֵ� TMP ����
    [SerializeField] private TMP_InputField inputFieldTMP;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_Dropdown playerRegionName;

    // ����ť��Ӧ������
    private const string creatingGameText = "Creating a Game...";
    private const string joiningGameText = "Joining a Game...";
    private const string destinationText = "Going to the Destination...";
    public static string roomName;
    public static string playerName;
    public static string region;

    public string gameSceneName;
    
    private CancellationTokenSource _tokenSource;

    // ��ʼ��
    void Start()
    {
        roomNameText.text = roomName;
        tmpText.gameObject.SetActive(false); // Ĭ������ TMP ����
        playerRegionName.onValueChanged.AddListener(OnRegionDropdownChanged);
    }
    
    private async void RefreshRegionDropdown() {
        _tokenSource = new CancellationTokenSource();

        var regions = await NetworkRunner.GetAvailableRegions(cancellationToken: _tokenSource.Token);
        playerRegionName.options.Clear();
        playerRegionName.AddOptions(regions.Select(reg => $"{reg.RegionCode} = {reg.RegionPing}").ToList());
    }

    /// <summary>
    /// TMP Dropdown ����״̬����ʱ��Ӧ.
    /// </summary>
    /// <param name="index">ѡ��Ĳ˵���</param>
    private void OnRegionDropdownChanged(int index)
    {
        switch (index)
        {
            case 0: region = "asia"; break;
            case 1: region = "eu"; break;
            case 2: region = "hk"; break;
            case 3: region = "us"; break;
            case 4: region = "usw"; break;
            case 5: region = "jp"; break;
            default: region = ""; break;
        }
    }

    /*private void Update()
    {
        RefreshRegionDropdown();
    }*/

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
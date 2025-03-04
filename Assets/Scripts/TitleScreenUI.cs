using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Linq;
using System.Collections.Generic;

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
    public static string Region;
    public static bool IsSpGame = false;

    public string gameSceneName;
    
    private CancellationTokenSource _tokenSource;

    // ��ʼ��
    void Start()
    {
        roomNameText.text = roomName;
        tmpText.gameObject.SetActive(false);
    }
    
    private List<RegionInfo> _regions; // 专门存储排序后的区域数据

    public async void RefreshRegionDropdown()
    {
        playerRegionName.onValueChanged.AddListener(index =>
        {
            // 每次选项更新时获取对应RegionCode
            Region = GetSelectedRegionCode();
            Debug.Log($"Player selected region: {Region}");
        });
        
        _tokenSource = new CancellationTokenSource();

        // 获取原始区域数据
        var regions = await NetworkRunner.GetAvailableRegions(cancellationToken: _tokenSource.Token);

        // 按Ping排序
        var sortedRegions = regions.OrderBy(reg => reg.RegionPing).ToList();

        // 更新_regions，确保与下拉框的顺序一致
        _regions = sortedRegions;

        // 清空并加载排序后的下拉按钮选项
        playerRegionName.options.Clear();
        playerRegionName.AddOptions(sortedRegions.Select(reg =>
        {
            string displayName = reg.RegionCode switch
            {
                "hk" => "Hong Kong",
                "us" => "United States",
                "eu" => "Europe",
                "asia" => "Asia",
                "au" => "Australia",
                "usw" => "US West",
                "uae" => "UAE",
                "tr" => "Turkey",
                "cae" => "Canada East",
                "ussc" => "US South Central",
                "jp" => "Japan",
                "in" => "India",
                "kr" => "Korea",
                "sa" => "South America",
                "cn" => "Mainland China",
                _ => reg.RegionCode
            };
            return new TMP_Dropdown.OptionData($"{displayName}. ping: {reg.RegionPing}");
        }).ToList());

        // 自动设置默认选项为第一个选项
        if (playerRegionName.options.Count > 0)
        {
            playerRegionName.value = 0; // 默认选中第一个（Ping最小的）
            Debug.Log($"Default selected region: {_regions[0].RegionCode}");
        }
    }

    public string GetSelectedRegionCode()
    {
        // 检查下拉框是否有选项
        if (playerRegionName.options.Count > 0)
        {
            int selectedIndex = playerRegionName.value;  // 当前选中项索引
            return _regions[selectedIndex].RegionCode; // 从_regions中获取RegionCode
        }

        return string.Empty; // 没有选项时返回空值
    }


    public void OnSinglePlayerButton()
    {
        IsSpGame = true;
        HandleButtonClick(creatingGameText);
    }

    /// <summary>
    /// ���¡��������䡱��ťʱ����
    /// </summary>
    public void OnCreateRoomButton()
    {
        IsSpGame = false;
        HandleButtonClick(creatingGameText);
    }

    /// <summary>
    /// ���¡����뷿�䡱��ťʱ����
    /// </summary>
    public void OnJoinRoomButton()
    {
        IsSpGame = false;
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
        try
        {
            Region = playerRegionName.options[playerRegionName.value].text;
        }
        catch
        {
            Region = string.Empty;
        }
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
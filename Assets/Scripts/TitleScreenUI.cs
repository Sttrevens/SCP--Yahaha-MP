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
    public static bool IsSpectator = false;

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
        // 当玩家在 UI 上切换下拉选项时，通过索引从 _regions 列表取得对应 RegionCode
        playerRegionName.onValueChanged.AddListener(index =>
        {
            // 下标 index 与 _regions[index] 匹配
            Region = _regions[index].RegionCode;
            Debug.Log($"玩家选择的区域: {Region}");
        });
    
        _tokenSource = new CancellationTokenSource();

        // 1. 获取可用的区域数据
        var regions = await NetworkRunner.GetAvailableRegions(cancellationToken: _tokenSource.Token);
    
        // 2. 按 Ping 值排序
        var sortedRegions = regions.OrderBy(reg => reg.RegionPing).ToList();
        _regions = sortedRegions; // 同步给全局 _regions，保证与下拉选项一一对应
    
        // 3. 构建 Dropdown 选项
        playerRegionName.options.Clear();
        playerRegionName.AddOptions(
            sortedRegions.Select(reg =>
            {
                // 将 RegionCode 转成人类可读的名称
                string displayName = reg.RegionCode switch
                {
                    "hk"   => "Hong Kong",
                    "us"   => "United States",
                    "eu"   => "Europe",
                    "asia" => "Asia",
                    "au"   => "Australia",
                    "usw"  => "US West",
                    "uae"  => "UAE",
                    "tr"   => "Turkey",
                    "cae"  => "Canada East",
                    "ussc" => "US South Central",
                    "jp"   => "Japan",
                    "in"   => "India",
                    "kr"   => "Korea",
                    "sa"   => "South America",
                    "cn"   => "Mainland China",
                    "ru"   => "Russia",
                    "rue"  => "Russia East",
                    "za"   => "South Africa",
                    _      => reg.RegionCode
                };
                // 此处只显示友好名称和 ping，不需要直接在文本中嵌入 RegionCode
                return new TMP_Dropdown.OptionData($"{displayName}. ping: {reg.RegionPing}");
            }).ToList()
        );

        // 4. 默认选中 Ping 最小的选项
        if (playerRegionName.options.Count > 0)
        {
            playerRegionName.value = 0;
            Debug.Log($"默认选择的区域: {_regions[0].RegionCode}");
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
            Region = _regions[playerRegionName.value].RegionCode;
        }
        catch
        {
            Region = string.Empty;
        }

        //SceneManager.LoadScene(gameSceneName);
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

    public void OnSpectatorButton()
    {
        IsSpectator = true;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
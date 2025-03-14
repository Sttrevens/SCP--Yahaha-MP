using UnityEngine;
using TMPro;

public class ToggleSelection : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "1280 x 800",
            "1920 x 1080",
            "3840 x 2160"
        });

        // 添加选项改变时的事件监听
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetResolution(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(1280, 800, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
            default:
                Debug.LogError("Invalid dropdown index");
                break;
        }
    }
}
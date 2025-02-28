/*using Fusion;
using UnityEngine;

public class PhotonRegionSetup : MonoBehaviour
{
    void Start()
    {
        var appSettings = PhotonAppSettings.Instance;
        
        // 设置固定区域为中国大陆（cn）
        appSettings.FixedRegion = "cn";

        // 确保 Photon 使用我们修改后的设置
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.ProvideInput = true; // 根据需要设置
            runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host
            });
        }
    }
}*/
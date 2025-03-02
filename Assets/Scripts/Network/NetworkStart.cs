using Fusion;
using UnityEngine;
using Fusion.Photon.Realtime;

public class NetworkStart : MonoBehaviour
{
    private FusionBootstrap bootstrap;

    public string roomName;

    private void Awake()
    {
        bootstrap = GetComponent<FusionBootstrap>();

        if (TitleScreenUI.roomName != null)
        {
            bootstrap.DefaultRoomName = TitleScreenUI.roomName;
            
            PhotonAppSettings.Global.AppSettings.FixedRegion = TitleScreenUI.region;
        }

        roomName = bootstrap.DefaultRoomName;
        bootstrap.StartSharedClient();
        
        AudioManager.Instance.StopBGM();
    }
}

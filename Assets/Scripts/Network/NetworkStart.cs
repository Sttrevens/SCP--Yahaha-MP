using Fusion;
using UnityEngine;
using Fusion.Photon.Realtime;

public class NetworkStart : MonoBehaviour
{
    private FusionBootstrap bootstrap;

    public string roomName;

    public bool isSinglePlayerMode;

    private void Awake()
    {
        bootstrap = GetComponent<FusionBootstrap>();

        if (TitleScreenUI.roomName != null)
        {
            bootstrap.DefaultRoomName = TitleScreenUI.roomName;
        }

        if (TitleScreenUI.Region != null)
        {
            PhotonAppSettings.Global.AppSettings.FixedRegion = TitleScreenUI.Region;
        }

        roomName = bootstrap.DefaultRoomName;

        isSinglePlayerMode = TitleScreenUI.IsSpGame;
        
        if (isSinglePlayerMode)
        {
            bootstrap.StartSinglePlayer();
        }
        else
        {
            bootstrap.StartSharedClient();
        }

        AudioManager.Instance.StopBGM();
    }
}

using Fusion;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    private FusionBootstrap bootstrap;

    private void Awake()
    {
        bootstrap = GetComponent<FusionBootstrap>();

        if (TitleScreenUI.roomName != null)
        {
            bootstrap.DefaultRoomName = TitleScreenUI.roomName;
        }
        bootstrap.StartSharedClient();
    }
}

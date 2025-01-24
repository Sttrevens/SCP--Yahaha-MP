using Fusion;
using UnityEngine;

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
        }

        roomName = bootstrap.DefaultRoomName;
        bootstrap.StartSharedClient();
    }
}

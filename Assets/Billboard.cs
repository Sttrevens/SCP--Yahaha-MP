using UnityEngine;
using Fusion;

public class Billboard : NetworkBehaviour
{
    private Camera mainCamera;
    private bool isBillboardEnabled = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null && isBillboardEnabled)
        {
            // 让物体始终朝向相机
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetBillboardEnabled(bool isEnabled)
    {
        isBillboardEnabled = isEnabled;
        gameObject.SetActive(isEnabled);

        SetPlayerName();
    }

    private void SetPlayerName()
    {
        GetComponent<PlayerNameTracker>().SetPlayerName();
    }
}
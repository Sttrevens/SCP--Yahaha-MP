using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    private bool isBillboardEnabled = true;
    public GameObject billboardText;

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
    
    public void SetBillboardEnabled(bool isEnabled)
    {
        isBillboardEnabled = isEnabled;
        billboardText.SetActive(isEnabled);
    }
}
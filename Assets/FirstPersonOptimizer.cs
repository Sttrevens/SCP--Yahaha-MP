using LPSurvivalEngine;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonOptimizer : MonoBehaviour
{
    [Header("Wieldable Manager")]
    public Transform cameraPosition;
    public Transform aimPosition;
    public Transform flashlightPosition;

    private void Start()
    {
        GameObject currentPlayerObject = GameObject.Find("CurrentPlayer");
        if (currentPlayerObject != null)
        {
            SkinnedMeshRenderer[] skinnedMeshRenderers = currentPlayerObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                // Set the renderer to only be invisible to the main camera
                renderer.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
                
                // Make sure the main camera's culling mask excludes the FirstPerson layer
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("FirstPerson"));
            }
        }
    }

    public void Wield()
    {
        WieldableManager.instance.cameraPositon = cameraPosition;
        WieldableManager.instance.AimPositon = aimPosition;
        WieldableManager.instance.flashlightPosition = flashlightPosition;
    }

    public Vector3 GetCameraForward()
    {
        return Camera.main.transform.forward;
    }

    public Quaternion GetCameraRotation()
    {
        return Camera.main.transform.rotation;
    }
}
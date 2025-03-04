using LPSurvivalEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FirstPersonOptimizer : MonoBehaviour
{
    [Header("Wieldable Manager")]
    public Transform cameraPosition;
    public Transform aimPosition;
    public Transform flashlightPosition;
    
    [Header("Shadow Casting Mode")]
    [SerializeField] private ShadowCastingMode shadowCastingMode;

    private void Start()
    {
        GameObject currentPlayerObject = GameObject.Find("CurrentPlayer");
        if (currentPlayerObject != null)
        {
            // Get both SkinnedMeshRenderer and MeshRenderer components
            SkinnedMeshRenderer[] skinnedMeshRenderers = currentPlayerObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] meshRenderers = currentPlayerObject.GetComponentsInChildren<MeshRenderer>();

            // Handle SkinnedMeshRenderers
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                /*// Set the renderer to only be invisible to the main camera
                renderer.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
                
                // Make sure the main camera's culling mask excludes the FirstPerson layer
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("FirstPerson"));*/

                renderer.shadowCastingMode = shadowCastingMode;
            }

            // Handle MeshRenderers
            foreach (MeshRenderer renderer in meshRenderers)
            {
                /*// Make sure the main camera's culling mask excludes the FirstPerson layer
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("FirstPerson"));
                if (renderer.transform.parent != null && renderer.transform.parent.GetComponent<CameraController>() != null)
                {
                    return;
                }
                renderer.gameObject.layer = LayerMask.NameToLayer("FirstPerson");*/
                
                renderer.shadowCastingMode = shadowCastingMode;
            }
        }
    }

    public void Wield()
    {
        WieldableManager.instance.cameraPositon = cameraPosition;
        WieldableManager.instance.aimPositon = aimPosition;
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
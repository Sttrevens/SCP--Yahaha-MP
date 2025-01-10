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
                renderer.enabled = false;
            }
        }
    }

    public void Wield()
    {
        WieldableManager.instance.cameraPositon = cameraPosition;
        WieldableManager.instance.AimPositon = aimPosition;
        WieldableManager.instance.flashlightPosition = flashlightPosition;
    }
}
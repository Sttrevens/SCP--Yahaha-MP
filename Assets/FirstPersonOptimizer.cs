using System.Collections.Generic;
using UnityEngine;

public class FirstPersonOptimizer : MonoBehaviour
{
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
}
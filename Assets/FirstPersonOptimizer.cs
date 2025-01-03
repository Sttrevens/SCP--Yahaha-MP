using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FirstPersonOptimizer : NetworkBehaviour
{
    private List<SkinnedMeshRenderer> allSkinnedMeshRenderers = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        GetAllSkinnedMeshRenderers(transform);

        if (HasStateAuthority)
        foreach (var renderer in allSkinnedMeshRenderers)
        {
            renderer.enabled = false;
        }
    }

    private void GetAllSkinnedMeshRenderers(Transform currentTransform)
    {
        SkinnedMeshRenderer renderer = currentTransform.GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            allSkinnedMeshRenderers.Add(renderer);
        }

        for (int i = 0; i < currentTransform.childCount; i++)
        {
            Transform child = currentTransform.GetChild(i);
            GetAllSkinnedMeshRenderers(child);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
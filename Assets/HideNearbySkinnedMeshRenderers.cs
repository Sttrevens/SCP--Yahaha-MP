using UnityEngine;

public class HideNearbySkinnedMeshRenderers : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            float distanceToCamera = Vector3.Distance(renderer.transform.position, transform.position);
            if (distanceToCamera < 2f)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCameraOptimizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject currentPlayerObject = GameObject.Find("CurrentPlayer");
        GameObject thisCameraMan = transform.root.gameObject;

        Camera thisCamera = GetComponent<Camera>();
        if (thisCamera == null)
        {
            Debug.LogError("No Camera component found on this object.");
            return;
        }

        if (thisCameraMan != null)
        {
            // Get both SkinnedMeshRenderer and MeshRenderer components under the highest parent object
            SkinnedMeshRenderer[] skinnedMeshRenderers = thisCameraMan.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] meshRenderers = thisCameraMan.GetComponentsInChildren<MeshRenderer>();

            // Handle SkinnedMeshRenderers
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                // Exclude this renderer from this camera's culling specifically
                thisCamera.cullingMask &= ~(1 << renderer.gameObject.layer);
            }

            // Handle MeshRenderers
            foreach (MeshRenderer renderer in meshRenderers)
            {
                // Exclude this renderer from this camera's culling specifically
                thisCamera.cullingMask &= ~(1 << renderer.gameObject.layer);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

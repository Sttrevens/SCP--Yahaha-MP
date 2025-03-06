using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class EyedSlimeEyeController : MonoBehaviour
{
    public JudaEyedSlimeRedController judaEyedSlimeRedController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VolumetricLightBeamAbstractBase>() != null)
        {
            Transform topMostParent = other.transform;
            while (topMostParent.parent != null)
            {
                topMostParent = topMostParent.parent;
            }
            judaEyedSlimeRedController.Ahhh(topMostParent.gameObject);
        }
    }
}

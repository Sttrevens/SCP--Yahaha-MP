using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ThirdPersonOptimizer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
        {
            if (fp_AimRoot != null)
            {
                TP_Aim();
            }
        }

        [HideInInspector] public Transform fp_AimRoot;

        private void TP_Aim()
        {
            transform.position = fp_AimRoot.position;
            transform.rotation = fp_AimRoot.rotation;
        }
}

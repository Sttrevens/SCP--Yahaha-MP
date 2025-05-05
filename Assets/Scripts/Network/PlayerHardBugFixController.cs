using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;

public class PlayerHardBugFixController : NetworkBehaviour
{
    private HealthSystem _networkTransform;
    
    public override void Spawned()
    {
        _networkTransform = GetComponent<HealthSystem>();
    }

    public override void FixedUpdateNetwork()
    {
        if (transform.position.y < -200)
        {
            _networkTransform.Rpc_Respawn();
        }
    }
}

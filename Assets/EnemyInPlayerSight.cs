using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyInPlayerSight : NetworkBehaviour
{
    [Networked] public bool isEnemyInSight { get; set; } = false;
    private void EnemyInSight()
    {
            if (gameObject.name == "CurrentPlayer")
            {
                var curCamera = Camera.main;
                var ghostConeObj = curCamera.transform.Find("GhostConeDetection");
                if (ghostConeObj != null) {
                    var coneDetection = ghostConeObj.GetComponent<ConeDetection>();
                    if (coneDetection != null) {
                        isEnemyInSight = coneDetection.hasTargetInView;
                    }
                }
                Debug.Log($"EnemyInSight: {isEnemyInSight}");
            }
            else
            {
                isEnemyInSight = false;
                Debug.Log("EnemyInSight: CurrentChasingPlayer is null or not the CurrentPlayer");
            }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_EnemyInSight()
    {
        EnemyInSight();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyInPlayerSight : NetworkBehaviour
{
    [Networked] public bool isEnemyInSight { get; set; } = false;
    public void EnemyInSight(GameObject CurrentChasingPlayer)
    {
            if (CurrentChasingPlayer != null && CurrentChasingPlayer.name == "CurrentPlayer")
            {
                var curCamera = Camera.main;
                Rpc_EnemyInSight(curCamera.GetComponentInChildren<ConeDetection>().hasTargetInView);
                Debug.Log($"EnemyInSight: {isEnemyInSight}");
            }
            else
            {
                Rpc_EnemyInSight(false);
                Debug.Log("EnemyInSight: CurrentChasingPlayer is null or not the CurrentPlayer");
            }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_EnemyInSight(bool isinSight)
    {
        isEnemyInSight = isinSight;
    }
}

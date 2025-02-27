using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyInPlayerSight : NetworkBehaviour
{
    [Networked] public bool isEnemyInSight { get; set; } = false;
    public void EnemyInSight(GameObject CurrentChasingPlayer)
    {
        if (HasStateAuthority)
        {
            if (CurrentChasingPlayer != null && CurrentChasingPlayer.name == "CurrentPlayer")
            {
                var curCamera = Camera.main;
                isEnemyInSight = curCamera.GetComponentInChildren<ConeDetection>().hasTargetInView;
                Debug.Log($"EnemyInSight: {isEnemyInSight}");
            }
            else
            {
                isEnemyInSight = false;
                Debug.Log("EnemyInSight: CurrentChasingPlayer is null or not the CurrentPlayer");
            }
        }
    }
}

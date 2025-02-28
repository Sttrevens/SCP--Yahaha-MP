using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Fusion;

public class EnemyDefend : NetworkBehaviour
{
    public Enemy enemy;
    public bool DefendState = false;
    public bool PlayerCanSeeEnemy = false;
    public GameObject CurrentChasingPlayer;

    public ChasingEnemy ChasingEnemyInstance;
    // [Header("防御动画相关变量")]
    private void Awake()
    {
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (ChasingEnemyInstance == null) ChasingEnemyInstance = enemy.GetComponent<ChasingEnemy>();

    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            CurrentChasingPlayer = ChasingEnemyInstance.targetPlayer;
            if (CurrentChasingPlayer != null)
            {
                CurrentChasingPlayer.GetComponent<EnemyInPlayerSight>().Rpc_EnemyInSight();
                PlayerCanSeeEnemy = CurrentChasingPlayer.GetComponent<EnemyInPlayerSight>().isEnemyInSight;
                if (CurrentChasingPlayer != null && PlayerCanSeeEnemy)
                {
                    DefendState = true;
                    enemy.SwitchState(new DefendState());
                }
            }
        }
    }
}

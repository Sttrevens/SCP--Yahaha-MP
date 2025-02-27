using System;
using System.Collections;
using System.Collections.Generic;
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
        CurrentChasingPlayer = ChasingEnemyInstance.targetPlayer;
        PlayerCanSeeEnemy = EnemyInPlayerSight();
        if (CurrentChasingPlayer != null && PlayerCanSeeEnemy)
        {
            DefendState = true;
            enemy.SwitchState(new DefendState());
        }
    }

    
    public bool EnemyInPlayerSight()
    {
        if (CurrentChasingPlayer != null && CurrentChasingPlayer.name == "CurrentPlayer")
        {
            var curCamera = Camera.main;
            return curCamera.GetComponentInChildren<ConeDetection>().hasTargetInView;
        }
        else return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public abstract class EnemyBaseState : IEnemyState
{
    public EnemyMovement EnemyMovement;
    public ChasingEnemy ChasingEnemy;
    public EnemyAttack EnemyAttack;
    public EnemySpell EnemySpell;
    
    public virtual void EnterState(Enemy enemy)
    {
        if (enemy.HasStateAuthority)
        {
            EnemyMovement = enemy.GetComponent<EnemyMovement>();
            ChasingEnemy = enemy.GetComponent<ChasingEnemy>();
            EnemyAttack = enemy.GetComponent<EnemyAttack>();
        }
    }
    public virtual void UpdateState(Enemy enemy) { }
    public virtual void ExitState(Enemy enemy) { }
}
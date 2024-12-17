using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EnemyBaseState : IEnemyState
{
    public virtual void EnterState(EnemyAI enemy) { }
    public virtual void UpdateState(EnemyAI enemy) { }
    public virtual void ExitState(EnemyAI enemy) { }
}
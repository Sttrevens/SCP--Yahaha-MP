using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public abstract class EnemyBaseState : NetworkBehaviour, IEnemyState
{
    public virtual void EnterState(EnemyAI enemy) { }
    public virtual void UpdateState(EnemyAI enemy) { }
    public virtual void ExitState(EnemyAI enemy) { }
}
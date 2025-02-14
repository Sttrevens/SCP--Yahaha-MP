using Fusion;
using UnityEngine;

public class EnemyAnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector]
    public int     AttackCount              { get; set; }
    public int     isChasing                { get; set; }
    public int     isPatrolling              { get; set; }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            AttackCount = 0;
            isChasing = 0;
            isPatrolling = 0;
        }
    }
}
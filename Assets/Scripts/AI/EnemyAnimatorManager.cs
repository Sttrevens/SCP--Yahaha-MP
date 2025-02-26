using Fusion;
using UnityEngine;

public class EnemyAnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector]
    public int     AttackCount              { get; set; }
    public bool     isChasing                { get; set; }
    public bool     isPatrolling              { get; set; }
    public int CastSpellCount { get; set; }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            AttackCount = 0;
            CastSpellCount = 0;
            isChasing = false;
            isPatrolling = false;
        }
    }
}
using Fusion;
using UnityEngine;

public class EnemyAnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector]
    public int     AttackCount              { get; set; }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            AttackCount = 0;
        }
    }
}
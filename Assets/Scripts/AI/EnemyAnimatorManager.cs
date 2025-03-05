using Fusion;
using UnityEngine;

public class EnemyAnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector] public int AttackCount { get; set; }
    [Networked, HideInInspector] public bool isChasing { get; set; }
    [Networked, HideInInspector] public bool isPatrolling { get; set; }
    [Networked, HideInInspector] public int CastSpellCount { get; set; }
    [Networked, HideInInspector] public bool spellingBool { get; set; }
    [Networked, HideInInspector] public bool isinSpecialState { get; set; }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            AttackCount = 0;
            CastSpellCount = 0;
            isChasing = false;
            isPatrolling = false;
            spellingBool = false;
            isinSpecialState = false;
        }
    }
}
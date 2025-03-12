using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector]
    public int     JumpCount              { get; set; }
    [Networked, HideInInspector]
    public float   Speed                  { get; set;}
    [Networked, HideInInspector] 
    public float   XAxis          { get; set; }
    [Networked, HideInInspector] 
    public float   ZAxis          { get; set; }
    [Networked, HideInInspector]
    public int   PickupCount              { get; set;}
    [Networked, HideInInspector]
    public int   ThrowCount               { get; set;}

    [Networked, HideInInspector]
    public int DieCount            { get; set;}
    [Networked, HideInInspector] 
    public int DyingCount          { get; set;}
    [Networked, HideInInspector]
    public bool IsHolding               { get; set;}
    [Networked, HideInInspector]
    public bool IsAiming               { get; set;}
    [Networked, HideInInspector]
    public bool IsTwerkDancing            { get; set;}
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            JumpCount = 0;
            PickupCount = 0;
            ThrowCount = 0;
            DieCount = 0;
            DyingCount = 0;
            IsHolding = false;
            IsAiming = false;
            IsTwerkDancing = false;
        }
    }
}
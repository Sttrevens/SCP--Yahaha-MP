using Fusion;
using UnityEngine;

public class AnimatorManager : NetworkBehaviour
{
    [Networked, HideInInspector]
    public int     JumpCount              { get; set; }
    [Networked, HideInInspector]
    public float   Speed                  { get; set;}
    [Networked, HideInInspector]
    public int   PickupCount              { get; set;}
    [Networked, HideInInspector]
    public int   WieldCount               { get; set;}

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            JumpCount = 0;
            PickupCount = 0;
        }
    }
}
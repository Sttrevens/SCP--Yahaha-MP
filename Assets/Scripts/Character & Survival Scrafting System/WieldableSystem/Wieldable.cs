using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class Wieldable : NetworkBehaviour
    {
        public virtual void OnAttackInput() { }

        public virtual void OnAltAttackInput() { }

        public PlayerRef player;
    }


}
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class Wieldable : NetworkBehaviour
    {
        public virtual void OnAttackInput(){}

        public virtual void OnAltAttackInput(){}

        public PlayerRef player;

        private void Update()
        {
            if (player == Runner.LocalPlayer)
            {


                // 只有拥有 StateAuthority 的客户端才负责同步物体的 Transform
                //if (HasStateAuthority)
                //{
                //    if (currentWieldable != null)
                //    {
                
                //    }
                //}
            }
        }

    }


}
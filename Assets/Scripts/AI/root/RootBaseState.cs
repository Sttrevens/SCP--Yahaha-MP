using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBaseState : MonoBehaviour
{
    public virtual void EnterState(Root enemy) { }
    public virtual void UpdateState(Root enemy) { }
    public virtual void ExitState(Root enemy) { }
}

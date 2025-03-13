using UnityEngine;
/// <summary>
/// 这个脚本只是处理一个状态的切换。
/// </summary>
public class IdleStateBehaviour : StateMachineBehaviour
{
    private RigController rigController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rigController == null)
            rigController = animator.GetComponent<RigController>();
        rigController?.SwitchToIdle(1.0f);
    }
}
using UnityEngine;
/// <summary>
/// 这个脚本只是处理一个状态的切换。
/// </summary>
public class AimStateBehaviour : StateMachineBehaviour
{
    public float switchSpeed = 4f;
    private RigController rigController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rigController == null)
            rigController = animator.GetComponent<RigController>();
        animator.SetBool("isHolding", false);
        rigController?.SwitchToAim(2.0f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rigController == null)
            rigController = animator.GetComponent<RigController>();

        rigController.SwitchToHipFire(2.0f);
    }
}
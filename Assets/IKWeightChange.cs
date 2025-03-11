using UnityEngine;

public class IKWeightController : StateMachineBehaviour
{
    public float targetEnterWeight = 1f;
    public float targetExitWeight = 0f;
    public float weightChangeSpeed = 2f;

    private RigController _rigController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_rigController == null)
            _rigController = animator.GetComponent<RigController>();

        if (_rigController != null)
            _rigController.SwitchToHipFire(2.0f);
        else
            Debug.LogError("未找到 IKWeightManager，请确保角色已挂载 IKWeightManager 脚本！");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_rigController == null)
            _rigController = animator.GetComponent<RigController>();

        if (_rigController != null)
            _rigController.SwitchToIdle(2.0f);
        else
            Debug.LogError("未找到 IKWeightManager，请确保角色已挂载 IKWeightManager 脚本！");
    }
}
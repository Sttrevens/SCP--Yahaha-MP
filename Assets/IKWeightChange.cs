using UnityEngine;

public class IKWeightController : StateMachineBehaviour
{
    public float targetEnterWeight = 1f;
    public float targetExitWeight = 0f;
    public float weightChangeSpeed = 2f;

    private IKWeightManager weightManager;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weightManager == null)
            weightManager = animator.GetComponent<IKWeightManager>();

        if (weightManager != null)
            weightManager.ChangeWeight(targetEnterWeight, weightChangeSpeed);
        else
            Debug.LogError("未找到 IKWeightManager，请确保角色已挂载 IKWeightManager 脚本！");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("退出状态");
        if (weightManager == null)
            weightManager = animator.GetComponent<IKWeightManager>();

        if (weightManager != null)
            weightManager.ChangeWeight(targetExitWeight, weightChangeSpeed/2);
        else
            Debug.LogError("未找到 IKWeightManager，请确保角色已挂载 IKWeightManager 脚本！");
    }
}
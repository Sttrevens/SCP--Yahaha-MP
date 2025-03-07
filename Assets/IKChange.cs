using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKChange : StateMachineBehaviour
{
    public Transform targetIKPosition; // 目标位置
    private Animator animator;
    // public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    // {
    //     this.animator = animator;
    //     targetIKPosition = animator.transform.Find("IKGoal").transform;
    //     Debug.Log("当前的目标是"+targetIKPosition.name);
    //     // 进入状态时启用IK
    //     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    //     animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    //     animator.SetIKPosition(AvatarIKGoal.RightHand, targetIKPosition.position);
    //     animator.SetIKRotation(AvatarIKGoal.RightHand, targetIKPosition.rotation);
    // }
    //
    // public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    // {
    //     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
    //     animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
    // }
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        targetIKPosition = animator.transform.Find("IKGoalIdle");
        Debug.Log("当前的目标是"+targetIKPosition.name);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1); // 权重设为1表示完全控制
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetIKPosition.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, targetIKPosition.rotation);
        Debug.Log("设置animator的");
    }
    
    // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     if (animator != null)
    //     {
    //         
    //         targetIKPosition = animator.transform.Find("IKGoal");
    //         Debug.Log("当前的目标是"+targetIKPosition.name);
    //         // 进入状态时启用IK
    //         animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    //         animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    //         animator.SetIKPosition(AvatarIKGoal.RightHand, targetIKPosition.position);
    //         animator.SetIKRotation(AvatarIKGoal.RightHand, targetIKPosition.rotation);
    //         Debug.Log("设置animator的");
    //     }
    //     else
    //     {
    //         Debug.Log("animator为空");
    //     }
    // }
    
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     // 退出状态时关闭IK
    //     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
    //     animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKAim : StateMachineBehaviour
{
    public Transform targetIKPosition; // 目标位置
    private Animator animator;
    
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        targetIKPosition = animator.transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head/IKGoalAim");
        // animator.transform.Find("IKGoalAim/HoldPos").position  = animator.transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head/IKGoalAim/HoldPosAim").position;
        animator.transform.Find("IKGoalIdle/HoldPos").position = new Vector3(0f,0f,0f);
        Debug.Log("当前的目标是"+targetIKPosition.name);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1); // 权重设为1表示完全控制d
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetIKPosition.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, targetIKPosition.rotation);
        Debug.Log("设置animator的");
    }

}

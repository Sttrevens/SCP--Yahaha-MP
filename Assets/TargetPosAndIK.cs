using UnityEngine;

public class HandIKAdjuster : MonoBehaviour
{
    [SerializeField] private Transform _rightHandTarget; // 拖入目标位置的Transform

    private void OnAnimatorIK(int layerIndex)
    {
        Animator animator = GetComponent<Animator>();
        if (layerIndex == 1)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1); // 权重设为1表示完全控制
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandTarget.rotation);
        }
        
    }
}
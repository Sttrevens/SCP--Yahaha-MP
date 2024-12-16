using UnityEngine;

public class CheckAnimatorLayersStates : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < animator.layerCount; i++)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(i);
            string layerName = animator.GetLayerName(i);
            string stateName = stateInfo.IsName("") ? "未识别的动画状态" : stateInfo.IsName("") ? "未识别的动画状态" : stateInfo.ToString();
            Debug.Log("在动画层 " + layerName + " (层索引为 " + i + ")，当前播放的动画是 " + stateName);
        }
    }
}
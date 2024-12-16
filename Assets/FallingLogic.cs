using UnityEngine;

public class FallingLogic : StateMachineBehaviour
{
    public delegate void FallingAnimationStartedEventHandler(GameObject triggeredObject);
    public static event FallingAnimationStartedEventHandler OnFallingAnimationStarted;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsFalling", true);

        if (OnFallingAnimationStarted != null)
        {
            Debug.Log("Dying Object: " + animator.gameObject);
            OnFallingAnimationStarted(animator.gameObject);
        }
    }
}

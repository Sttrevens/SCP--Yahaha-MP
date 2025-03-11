using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebugMonitor : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private InputAction sprintAction;
    
    [SerializeField] private string actionName;

    private void OnEnable()
    {
        // 假设场景里已经有一个 PlayerInput
        // 并在其中配好了一个名为 "Sprint" 的 Action
        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null)
        {
            sprintAction = playerInput.actions.FindAction(actionName);
            if (sprintAction != null)
            {
                // 监控三种阶段：开始按下、执行中、取消/松开
                sprintAction.started += OnSprintStarted;
                sprintAction.performed += OnSprintPerformed;
                sprintAction.canceled += OnSprintCanceled;
                sprintAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (sprintAction != null)
        {
            sprintAction.started -= OnSprintStarted;
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;

            sprintAction.Disable();
        }
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        Debug.Log(actionName + " Started: 按键刚被按下");
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        Debug.Log(actionName + " Performed: 当前值={context.ReadValue<float>()}");
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        Debug.Log(actionName + " Canceled: 按键松开或其他终止");
    }
}
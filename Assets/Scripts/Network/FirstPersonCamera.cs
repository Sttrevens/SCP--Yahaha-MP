using System;
using UnityEngine;
using UnityEngine.InputSystem; // 新 Input System 命名空间
using UnityEngine.InputSystem.LowLevel; 
using LPSurvivalEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float Height = 0.7f;
    public float MouseSensitivity = 10f;

    public bool isCameraLocked = false;

    // 记录俯仰角、水平角
    public float verticalRotation;
    public float horizontalRotation;
    
    // 用来获取场景里带有 PlayerInput 的对象
    [SerializeField] private PlayerInput playerInput;
    private InputAction lookAction;

    private void OnEnable()
    {
        // 1. 从 PlayerInput 里找到你配置好的 "Look" Action
        if (playerInput != null)
        {
            lookAction = playerInput.actions.FindAction("Look");
            if (lookAction != null)
            {
                lookAction.Enable(); // 确保启用
            }
        }
    }

    private void OnDisable()
    {
        // 脚本禁用时，禁用 Look Action（可选）
        if (lookAction != null)
        {
            lookAction.Disable();
        }
    }

    private void Update()
    {
        if (Target == null) return;
        
        transform.SetParent(Target);
        transform.localPosition = new Vector3(0, Height, 0);
    }

    void LateUpdate()
    {
        if (Target == null) return;

        // 保持相机跟随角色
        //transform.position = Target.position + new Vector3(0, Height, 0);

        // 与原脚本相同：只有在 cursor 未锁定时才旋转
        isCameraLocked = !PlayerController.instance.cursor;
        if (isCameraLocked)
            return;

        // 2. 使用新 Input System 的 Look Action 取值
        Vector2 lookDelta = Vector2.zero;
        if (lookAction != null)
        {
            lookDelta = lookAction.ReadValue<Vector2>();
        }
        
        // 将取到的 x / y 分别用于水平旋转和垂直旋转
        float mouseX = lookDelta.x;
        float mouseY = lookDelta.y;

        // 3. 计算俯仰角并加以限制
        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        // 4. 计算水平旋转
        horizontalRotation += mouseX * MouseSensitivity;

        // 5. 更新相机最终旋转
        //transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        //transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
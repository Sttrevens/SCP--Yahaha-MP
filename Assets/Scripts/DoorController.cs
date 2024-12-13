using LPSurvivalEngine;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public bool isDoubleSwing = false; // 是否允许双开
    public float openAngle = 90f; // 开门的目标角度绝对值（双开时无方向性）
    public float closeAngle = 0f; // 关门的目标角度
    public float rotationSpeed = 2f; // 旋转速度
    public bool isOpen = false; // 当前门的状态（开/关）

    private Quaternion closeRotation; // 关门时的目标旋转
    private Quaternion targetRotation; // 当前目标旋转

    private void Start()
    {
        // 初始化旋转状态
        closeRotation = transform.rotation;
        targetRotation = closeRotation; // 初始状态为关闭
    }

    private void Update()
    {
        // 平滑旋转到目标角度
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ToggleDoor(Vector3 interactorPosition)
    {
        if (!isDoubleSwing)
        {
            // 单向开门
            isOpen = !isOpen;
            targetRotation = isOpen
                ? Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f)) // 开门
                : closeRotation; // 关门
        }
        else
        {
            // 双向开门，根据玩家位置判断方向
            Vector3 doorForward = transform.forward; // 门的正前方
            Vector3 toInteractor = (interactorPosition - transform.position).normalized;

            float dot = Vector3.Dot(doorForward, toInteractor); // 判断玩家位于门的哪一侧
            float direction = dot > 0 ? 1f : -1f; // 正前方为1，背后为-1

            isOpen = !isOpen;
            targetRotation = isOpen
                ? Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle * direction, 0f)) // 根据方向开门
                : closeRotation; // 关门
        }
    }

    public string GetInteractText()
    {
        return isOpen ? "Close" : "Open";
    }

    public void OnInteract()
    {
        // 获取玩家交互的方向
        Vector3 interactionDirection = (Camera.main.transform.position - transform.position).normalized;
        Debug.Log("Interaction Direction: " + interactionDirection);
        ToggleDoor(interactionDirection);
    }
}
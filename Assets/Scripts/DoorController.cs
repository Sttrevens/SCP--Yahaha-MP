using LPSurvivalEngine;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public bool isDoubleSwing = false; // 是否允许双开
    public float openAngle = 90f; // 开门的目标角度绝对值（双开时无方向性）
    public float closeAngle = 0f; // 关门的目标角度
    public float rotationSpeed = 2f; // 旋转速度
    public bool isOpen = false; // 当前门的状态（开/关）
    public bool isLocked = false; // 门是否上锁
    public GameObject doorModel; // 挂载的门模型物体

    private Quaternion closeRotation; // 关门时的目标旋转
    private Quaternion targetRotation; // 当前目标旋转
    private Vector3 initialPosition; // 门的初始位置，用于shake效果

    private bool isTriedLockedDoor = false;

    private void Start()
    {
        // 初始化旋转状态
        closeRotation = transform.rotation;
        targetRotation = closeRotation; // 初始状态为关闭
        initialPosition = transform.position; // 保存门的初始位置
    }

    private void Update()
    {
        // 平滑旋转到目标角度
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // 检测门模型是否被摧毁
        if (doorModel == null)
        {
            Destroy(gameObject); // 门模型被摧毁后销毁整个门
        }
    }

    public void ToggleDoor(Vector3 interactorPosition)
    {
        if (isLocked)
        {
            // 如果门上锁，触发shake效果
            StartCoroutine(ShakeDoor());
            return;
        }

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
        return isTriedLockedDoor ? "Locked" : (isOpen ? "Close" : "Open");
    }

    public void OnInteract()
    {
        // 获取玩家交互的方向
        Vector3 interactionDirection = (Camera.main.transform.position - transform.position).normalized;
        Debug.Log("Interaction Direction: " + interactionDirection);
        ToggleDoor(interactionDirection);
    }

    private System.Collections.IEnumerator ShakeDoor()
    {
        isTriedLockedDoor = true;

        // 门的快速震动效果
        float duration = 0.3f; // 震动持续时间
        float magnitude = 0.01f; // 震动幅度

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 生成随机的震动偏移
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;
            transform.position = initialPosition + new Vector3(randomOffset.x, 0, randomOffset.z);

            yield return null;
        }

        // 恢复到原始位置
        transform.position = initialPosition;
    }
}

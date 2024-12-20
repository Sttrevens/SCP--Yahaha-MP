using UnityEngine;

public class IngameCanvasCoordinator : MonoBehaviour
{
    public float detectionRange = 100f;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        bool isPositionValid = false;
        while (!isPositionValid)
        {
            // 先随机生成一个新位置
            Vector3 newPosition = GenerateRandomPosition();
            // 设置新位置的Y坐标保持不变，使用当前的Y坐标
            newPosition.y = rectTransform.position.y;
            rectTransform.position = newPosition;
            // 检查新位置是否满足方圆范围内只有自己
            isPositionValid = IsAloneInRange();
        }
    }

    private Vector3 GenerateRandomPosition()
    {
        // 获取当前Canvas所在的平面（这里假设在XY平面上，根据实际需求可调整）
        Vector3 currentPosition = rectTransform.position;
        float randomX = Random.Range(-detectionRange, detectionRange);
        float randomZ = Random.Range(-detectionRange, detectionRange);
        return new Vector3(currentPosition.x + randomX, currentPosition.y, currentPosition.z + randomZ);
    }

    private bool IsAloneInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(rectTransform.position, detectionRange);
        int count = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("In-game Screen Canvas") && collider.gameObject != gameObject)
            {
                // 如果有其他相同tag且不是自己的Canvas，数量加1
                count++;
            }
        }
        return count == 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputRelaySource : MonoBehaviour
{
    [SerializeField] LayerMask RaycastMask = ~0; // 用于过滤射线检测的层
    [SerializeField] float RaycastDistance = 15f; // 射线的最大检测距离
    [SerializeField] UnityEvent<Vector2> OnCursorInput = new UnityEvent<Vector2>(); // 输入事件

    Camera cam; // 主摄像机

    void Start()
    {
        cam = Camera.main; // 获取主摄像机
    }

    void Update()
    {
        // 从屏幕中心点发射射线
        Ray centerRay = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // 用于存储射线的碰撞结果
        RaycastHit hitResult;
        if (Physics.Raycast(centerRay, out hitResult, RaycastDistance, RaycastMask, QueryTriggerInteraction.Ignore))
        {
            // 如果射线未击中当前物体，直接返回
            if (hitResult.collider.gameObject != gameObject)
                return;

            // 如果射线击中目标，调用 OnCursorInput 事件，传递归一化纹理坐标
            OnCursorInput.Invoke(hitResult.textureCoord);
        }
    }
}

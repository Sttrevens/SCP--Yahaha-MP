using UnityEngine;

public class CenterRayChecker : MonoBehaviour
{
    [SerializeField] private float maxDistance = 50f;    // 射线的最大检测距离
    private Billboard lastBillboard = null;             // 记录上一次启用的Billboard

    public void FixedUpdate()
    {
        // 1. 从屏幕中心发射射线（(0.5f,0.5f)表示屏幕中心）
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // 2. 使用带距离限制的Physics.Raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // 尝试在命中的物体或其子物体上获取Billboard组件
            Billboard billboard = hit.transform.GetComponent<PlayerData>().billboard;

            if (billboard != null)
            {
                // 若命中新目标，先关闭上一次的Billboard
                if (lastBillboard != billboard)
                {
                    if (lastBillboard != null)
                    {
                        lastBillboard.Rpc_SetBillboardEnabled(false);
                    }
                    billboard.Rpc_SetBillboardEnabled(true);
                    lastBillboard = billboard;
                }
            }
            else
            {
                // 命中物体不存在Billboard时，关闭上一次的并清空
                if (lastBillboard != null)
                {
                    lastBillboard.Rpc_SetBillboardEnabled(false);
                    lastBillboard = null;
                }
            }
        }
        else
        {
            // 没有命中任何物体时，若上一次Billboard还在启用则将其关闭
            if (lastBillboard != null)
            {
                lastBillboard.Rpc_SetBillboardEnabled(false);
                lastBillboard = null;
            }
        }
    }
}
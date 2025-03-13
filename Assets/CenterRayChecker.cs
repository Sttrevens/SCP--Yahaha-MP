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

        Debug.Log("发射射线检测...");

        // 2. 使用带距离限制的Physics.Raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log($"射线命中: {hit.transform.name}");

            // 尝试在命中的物体或其子物体上获取Billboard组件
            Billboard billboard = hit.transform.GetComponent<PlayerData>()?.billboard;

            if (billboard != null && hit.transform.gameObject.name != "Currentplayer")
            {
                Debug.Log($"命中目标具有Billboard: {billboard.name}");

                // 若命中新目标，先关闭上一次的Billboard
                if (lastBillboard != billboard)
                {
                    if (lastBillboard != null)
                    {
                        Debug.Log($"关闭上一次的Billboard: {lastBillboard.name}");
                        lastBillboard.SetBillboardEnabled(false);
                    }
                    Debug.Log($"启用新的Billboard: {billboard.name}");
                    billboard.SetBillboardEnabled(true);
                    lastBillboard = billboard;
                }
            }
            else
            {
                Debug.Log("命中的目标没有Billboard组件");

                // 命中物体不存在Billboard时，关闭上一次的并清空
                if (lastBillboard != null)
                {
                    Debug.Log($"关闭上一次的Billboard: {lastBillboard.name}");
                    lastBillboard.SetBillboardEnabled(false);
                    lastBillboard = null;
                }
            }
        }
        else
        {
            Debug.Log("射线未命中任何物体");

            // 没有命中任何物体时，若上一次Billboard还在启用则将其关闭
            if (lastBillboard != null)
            {
                Debug.Log($"关闭上一次的Billboard: {lastBillboard.name}");
                lastBillboard.SetBillboardEnabled(false);
                lastBillboard = null;
            }
        }
    }
}
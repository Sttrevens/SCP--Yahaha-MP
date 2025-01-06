using UnityEngine;

public class SPAnimatorManager : MonoBehaviour
{
    // 移除网络相关特性，改为普通的字段
    public int JumpCount { get; set; }
    public float Speed { get; set; }
    public int PickupCount { get; set; }
    public int WieldCount { get; set; }
    public int TwoHandWieldCount { get; set; }

    private void Awake()
    {
        // 在Awake中进行初始化，模拟原本在网络环境下有状态权限时的初始化操作
        JumpCount = 0;
        PickupCount = 0;
        WieldCount = 0;
        TwoHandWieldCount = 0;
    }
}
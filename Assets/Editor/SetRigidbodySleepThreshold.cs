using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SetRigidbodySleepThreshold : Editor
{
    [MenuItem("Tools/Set Rigidbody Sleep Threshold")]
    static void SetSleepThreshold()
    {
        // 查找场景中所有的Rigidbody组件
        Rigidbody[] rigidbodies = Object.FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            // 设置休眠阈值，这里设置为0.1，你可以根据需要修改这个值
            rb.sleepThreshold = 0.1f;
        }
        Debug.Log("已为所有刚体设置休眠阈值。");
    }
}
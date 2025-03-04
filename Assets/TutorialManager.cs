using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Fusion;

public class TutorialManager : NetworkBehaviour
{
    // 在Inspector面板把 Waypoint_Indicator 元素拖进来
    [FormerlySerializedAs("waypoints")] public List<GameObject> tutorialUIObjects;

    private int currentIndex = 0;

    void Start()
    {
        // 如有多个waypoint，先启用第一个，禁用其余
        if (tutorialUIObjects != null && tutorialUIObjects.Count > 0)
        {
            for (int i = 0; i < tutorialUIObjects.Count; i++)
            {
                if (i == 0)
                {
                    tutorialUIObjects[i].SetActive(true);
                }
                else
                {
                    tutorialUIObjects[i].SetActive(false);
                }
            }
            currentIndex = 0;
        }
    }
    
    public void EnableNextTutorialUI()
    {
        RpcEnableNextTutorialUI();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcEnableNextTutorialUI()
    {
        if (tutorialUIObjects == null || tutorialUIObjects.Count == 0) return;

        // 先禁用当前
        tutorialUIObjects[currentIndex].SetActive(false);

        // 下标递增
        currentIndex++;

        // 如果越界，可根据需求决定是循环回到0，还是就停止
        if (currentIndex >= tutorialUIObjects.Count)
        {
            // 可以选择循环或停止，这里演示停止：
            currentIndex = tutorialUIObjects.Count - 1;
            return;
        }

        // 启用下一个
        tutorialUIObjects[currentIndex].SetActive(true);
    }
    
}
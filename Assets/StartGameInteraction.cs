using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Globalization;

public class StartGameInteraction : NetworkBehaviour, IInteractable
{
    public string gameSceneName;

    private bool isSceneLoading = false;  // 标志位，防止反复加载场景

    // 获取交互文本
    public string GetInteractText()
    {
        return "Set Destination";
    }

    // 交互逻辑，触发场景切换
    public void OnInteract()
    {
        if (!isSceneLoading)  // 确保场景加载标志为 false 时才触发
        {
            isSceneLoading = true;  // 设置标志，表示正在加载场景
            // 调用 Rpc 同步场景加载
            RPC_LoadScene(gameSceneName);
        }
    }

    // 通过 RPC 同步场景加载
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_LoadScene(string sceneName)
    {
        // 确保只加载目标场景
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            // 加载目标场景
            SceneManager.LoadScene(sceneName);
        }
    }

    // 在场景加载后，重置标志
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 当新场景加载完成后，重置标志
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneLoading = false;  // 场景加载完成后，重置标志
    }
}

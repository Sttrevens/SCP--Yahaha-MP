using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PlayerBarrageInput : NetworkBehaviour
{
    [Tooltip("玩家输入弹幕的InputField")]
    [SerializeField] private TMP_InputField barrageInputField;
    
    private PlayerData localPlayerData;
    
    private void Start()
    {
        // 设置输入框的回车事件
        barrageInputField.onSubmit.AddListener(OnBarrageSubmitted);
        
        // 确保在输入后清空输入框
        barrageInputField.onEndEdit.AddListener(ClearInputAfterSubmit);
        
        // 尝试获取本地玩家
        StartCoroutine(FindLocalPlayer());
    }
    
    private IEnumerator FindLocalPlayer()
    {
        // 等待玩家生成
        yield return new WaitForSeconds(1.0f);
        
        // 尝试查找玩家
        PlayerData[] allPlayers = FindObjectsOfType<PlayerData>();
        
        foreach (var player in allPlayers)
        {
            if (player.name == "CurrentPlayer")
            {
                localPlayerData = player;
                Debug.Log("找到本地玩家: " + localPlayerData.PlayerName);
                break;
            }
        }
        
        if (localPlayerData == null)
        {
            Debug.LogWarning("未找到本地玩家，将在1秒后重试");
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(FindLocalPlayer());
        }
    }

    private void OnBarrageSubmitted(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;
        
        if (BarrageUI.instance != null)
        {
            // 获取玩家名字
            string playerName = "匿名用户";
            if (localPlayerData != null && !string.IsNullOrEmpty(localPlayerData.PlayerName))
            {
                playerName = localPlayerData.PlayerName;
            }
            
            // 发送RPC给所有客户端，由Master Client处理
            if (localPlayerData != null && localPlayerData.Object != null)
            {
                RPC_PlayerBarrageRequest(message, playerName);
            }
            
            // 清空输入框
            barrageInputField.text = string.Empty;
        }
        else
        {
            Debug.LogError("找不到弹幕UI实例");
        }
    }
    
    private void ClearInputAfterSubmit(string message)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            barrageInputField.text = string.Empty;
        }
    }
    
    // 所有客户端可以调用，发给所有客户端
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerBarrageRequest(string message, string playerName)
    {
        // 只在拥有StateAuthority的客户端处理（Master Client）
        if (!Object.HasStateAuthority) return;
        
        if (BarrageUI.instance != null)
        {
            // 创建玩家弹幕项
            UserName userName = new UserName { id = 999, nickName = playerName };
            int userNameIndex = -1;
            
            // 查找玩家名称是否存在于UserName数组中
            if (UserNameClass.userName != null)
            {
                for (int i = 0; i < UserNameClass.userName.Length; i++)
                {
                    if (UserNameClass.userName[i].nickName == playerName)
                    {
                        userNameIndex = i;
                        break;
                    }
                }
            }
            
            // 如果找不到，则创建一个随机索引
            if (userNameIndex == -1 && UserNameClass.userName != null && UserNameClass.userName.Length > 0)
            {
                userNameIndex = 0; // 使用第一个用户名
                playerName = UserNameClass.userName[userNameIndex].nickName;
            }
            
            // 创建弹幕项
            BarrageItemJson playerBarrage = new BarrageItemJson
            {
                index = userNameIndex >= 0 ? userNameIndex : 0,
                desc = message
            };
            
            // 处理插入弹幕
            InsertPlayerBarrageToBarrageUI(playerBarrage, localPlayerData.PlayerName);
        }
    }
    
    // 在Master Client上执行插入玩家弹幕
    private void InsertPlayerBarrageToBarrageUI(BarrageItemJson playerBarrage, string username)
    {
        if (BarrageUI.instance == null) return;
    
        if (BarrageUI.instance.isStop || BarrageUI.instance.baragesArr == null || BarrageUI.instance.baragesArr.Length == 0)
        {
            // 如果弹幕系统当前没有运行，直接设置当前弹幕并创建
            BarrageUI.instance.curBarrage = playerBarrage;
            BarrageUI.instance.userNameText = username;
            BarrageUI.instance.RPC_CreateItem();
        }
        else
        {
            // 否则使用现有的插入方法
            BarrageUI.instance.InsertPlayerBarrage(playerBarrage, username);
        }
    }

}
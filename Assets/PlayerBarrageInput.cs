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
    private Coroutine findPlayerCoroutine;
    private bool isSearchingPlayer = false;
    
    private void Start()
    {
        // 设置输入框的回车事件
        barrageInputField.onSubmit.AddListener(OnBarrageSubmitted);
        
        // 确保在输入后清空输入框
        barrageInputField.onEndEdit.AddListener(ClearInputAfterSubmit);
        
        // 启动协程，但保存引用
        StartFindingLocalPlayer();
    }
    
    // 网络对象生成时调用（在Start之后）
    public override void Spawned()
    {
        base.Spawned();
        
        // 确保在网络连接后也尝试找到本地玩家
        Debug.Log("Spawned被调用，确保玩家查找协程在运行");
        StartFindingLocalPlayer();
    }
    
    // 重写OnEnable确保组件启用时协程继续运行
    private void OnEnable()
    {
        StartFindingLocalPlayer();
    }
    
    // 安全地启动查找玩家的协程
    private void StartFindingLocalPlayer()
    {
        if (isSearchingPlayer)
        {
            Debug.Log("已经在查找本地玩家中，不需要重新启动协程");
            return;
        }
        
        Debug.Log("启动查找本地玩家的协程");
        isSearchingPlayer = true;
        
        // 如果之前的协程存在，先停止它
        if (findPlayerCoroutine != null)
            StopCoroutine(findPlayerCoroutine);
        
        // 启动新的协程并保存引用
        findPlayerCoroutine = StartCoroutine(FindLocalPlayerPersistent());
    }
    
    // 更加健壮的查找本地玩家协程
    private IEnumerator FindLocalPlayerPersistent()
    {
        Debug.Log("开始持久化查找本地玩家...");
        int attempts = 0;
        
        // 持续尝试直到找到本地玩家
        while (localPlayerData == null && isSearchingPlayer)
        {
            attempts++;
            Debug.Log($"查找本地玩家尝试 #{attempts}");
            
            // 等待一小段时间
            yield return new WaitForSeconds(1.0f);
            
            // 尝试查找玩家
            PlayerData[] allPlayers = FindObjectsOfType<PlayerData>();
            Debug.Log($"当前场景中找到 {allPlayers.Length} 个玩家");
            
            foreach (var player in allPlayers)
            {
                // 找到名为CurrentPlayer的对象，这是根据你的原始逻辑
                if (player.name == "CurrentPlayer")
                {
                    localPlayerData = player;
                    Debug.Log($"成功找到本地玩家: {localPlayerData.PlayerName}");
                    
                    // 打印更多关于玩家的信息
                    Debug.Log($"本地玩家ID: {localPlayerData.GetInstanceID()}, Object: {(localPlayerData.Object != null ? "有效" : "无效")}");
                    
                    // 通知UI更新或执行其他初始化
                    OnLocalPlayerFound();
                    
                    // 找到后跳出循环
                    break;
                }
            }
            
            // 如果找到了，退出协程
            if (localPlayerData != null)
            {
                Debug.Log("本地玩家已找到，查找协程结束");
                isSearchingPlayer = false;
                yield break;
            }
            
            Debug.LogWarning("未找到本地玩家，将在1秒后重试");
            
            // 检查组件和GameObject状态
            if (!this.enabled || !this.gameObject.activeInHierarchy)
            {
                Debug.LogError("PlayerBarrageInput组件被禁用或GameObject不活跃，协程可能会意外终止");
            }
        }
    }
    
    // 找到本地玩家后的处理
    private void OnLocalPlayerFound()
    {
        Debug.Log("本地玩家已找到，初始化弹幕功能");
        // 这里可以添加任何需要在找到本地玩家后执行的逻辑
    }

    // 确保在组件被禁用或销毁时清理状态
    private void OnDisable()
    {
        if (findPlayerCoroutine != null)
        {
            StopCoroutine(findPlayerCoroutine);
            findPlayerCoroutine = null;
        }
        isSearchingPlayer = false;
        Debug.Log("PlayerBarrageInput被禁用，停止查找玩家协程");
    }
    
    private void OnDestroy()
    {
        if (findPlayerCoroutine != null)
        {
            StopCoroutine(findPlayerCoroutine);
            findPlayerCoroutine = null;
        }
        isSearchingPlayer = false;
        Debug.Log("PlayerBarrageInput被销毁，停止查找玩家协程");
    }

    private void OnBarrageSubmitted(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Debug.Log("收到空弹幕，已忽略");
            return;
        }
    
        if (BarrageUI.instance != null)
        {
            Debug.Log("开始处理弹幕消息: " + message);
        
            // 获取玩家名字
            string playerName = "匿名用户";
            if (localPlayerData != null && !string.IsNullOrEmpty(localPlayerData.PlayerName))
            {
                playerName = localPlayerData.PlayerName;
                Debug.Log("使用玩家名称: " + playerName);
            }
            else
            {
                Debug.Log("未找到玩家数据或玩家名称为空,使用默认名称: " + playerName);
            }
        
            // 发送RPC给所有客户端，由Master Client处理
            if (localPlayerData != null && localPlayerData.Object != null)
            {
                Debug.Log("发送弹幕RPC请求");
                RPC_PlayerBarrageRequest(message, playerName);
            }
            else
            {
                Debug.LogWarning("本地玩家对象无效，无法发送RPC");
            }
        
            // 清空输入框
            barrageInputField.text = string.Empty;
            Debug.Log("已清空输入框");
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
        Debug.Log("Master Client Handling Player Barrage Request: " + message + " from " + playerName + "");
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
using System;
using Fusion;
using LPSurvivalEngine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour,INetworkRunnerCallbacks
{
    public List<GameObject> PlayerPrefabsPool;
    public Transform spawnPoint;
    public GameObject spectatorPanel;

    // 只在本地维护对生成的播放器Prefab的引用，方便离开时回收
    private Dictionary<PlayerRef, GameObject> assignedPrefabs = new Dictionary<PlayerRef, GameObject>();

    // 用于保存原始列表（不希望被删除的备份）
    private List<GameObject> originalPool;
    
    #region callbacks
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.LogError("玩家加入");
        PlayerJoined(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.LogError("玩家离开");
        PlayerLeft(runner, player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
       
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogError("玩家离开");

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    #endregion
    public void PlayerJoined(NetworkRunner Runner, PlayerRef player)
    {
        Debug.Log("回调");
        if (TitleScreenUI.IsSpectator)
        {
            if (Camera.main != null)
                Camera.main.GetComponent<AudioListener>().enabled = false;
            spectatorPanel.SetActive(true);
            return;
        }
        
        // 当前加入的玩家是第几个 => chosenIndex
        int chosenIndex = Runner.ActivePlayers.Count() - 1;
        
        Debug.Log("original pool:" + originalPool.Count + " But Player pool: " + PlayerPrefabsPool.Count);

        if (Runner.ActivePlayers.Count() == 1)
        {
            originalPool = new List<GameObject>(PlayerPrefabsPool);
        }
        
        // 判断 index 是否在可用范围内
        if (chosenIndex < 0 || chosenIndex >= originalPool.Count)
        {
            Debug.LogWarning("No valid prefab index in the local pool.");
            return;
        }

        // 仅在本地玩家加入时进行处理
        if (player == Runner.LocalPlayer)
        {
            // 取出对应的Prefab并从本地池子中移除
            GameObject chosenPrefab = originalPool[chosenIndex];
            originalPool.RemoveAt(chosenIndex);

            // 验证 spawnPoint 是否正确
            Debug.Log($"Spawning at Position: {spawnPoint.position}, Rotation: {spawnPoint.rotation}");
            if (chosenPrefab == null)
            {
                Debug.LogError("playerPrefab 为空，请在 Inspector 里赋值！");
                return;
            }
            // 在服务器下进行 Spawn 操作
            NetworkObject plObject = Runner.Spawn(chosenPrefab, spawnPoint.position, spawnPoint.rotation, player);
            //这个过程是个异步过程
            Runner.SetPlayerObject(player, plObject);
            // Debug 新实例的实际位置
            Debug.Log($"Spawned Object Position: {plObject.transform.position}, Rotation: {plObject.transform.rotation}");

            // 安全的强制更新位置
            plObject.GetComponent<NetworkTransform>()?.Teleport(spawnPoint.position, spawnPoint.rotation);

            assignedPrefabs[player] = chosenPrefab;

            // 设置默认名字为 CurrentPlayer，用于调试
            plObject.name = "CurrentPlayer";

            // 以下代码为原逻辑
            GameObject.Find("Inventory").GetComponent<Inventory>().dropPosition =
                plObject.transform.Find("DropBox").transform;
            GameObject.Find("WieldManager").GetComponent<WieldableManager>().wieldablesPosition =
                plObject.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
        }
    }

    public void PlayerLeft(NetworkRunner Runner, PlayerRef player)
    {
        // 如果玩家名下有Prefab的引用，则将其归还至本地池子并 Despawn
        if (assignedPrefabs.TryGetValue(player, out var usedPrefab))
        {
            originalPool.Add(usedPrefab);
            assignedPrefabs.Remove(player);

            var netObj = Runner.GetPlayerObject(player);
            if (netObj != null)
            {
                Runner.Despawn(netObj);
            }
        }
    }
}
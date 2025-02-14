using Fusion;
using LPSurvivalEngine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    // 将单个Prefab改为一个池子
    public List<GameObject> PlayerPrefabsPool;
    public Transform spawnPoint;

    // 记录分配给玩家的Prefab，以便离开时归还
    private Dictionary<PlayerRef, GameObject> assignedPrefabs = new Dictionary<PlayerRef, GameObject>();

    // 如果需要，你也可以用一个字典来记录该玩家对应的Prefab下标
    private Dictionary<PlayerRef, int> assignedPrefabIndices = new Dictionary<PlayerRef, int>();

    [Networked, Capacity(4)]
    public NetworkLinkedList<int> characterMaterialIndex { get; }
    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsSharedModeMasterClient)
        {
            foreach (int i in Enumerable.Range(0, 4))
            {
                characterMaterialIndex.Add(i);
            }
        }

        // 先检查全局同步的 characterMaterialIndex，如果没有可用下标，则提示
        if (characterMaterialIndex.Count == 0)
        {
            Debug.LogWarning("No more available player prefabs in the pool via synced index.");
            return;
        }

        // 如果有 State Authority，则从全局列表里减去一个可用下标
        // 这里为了示例，直接取第 0 个。实际逻辑可按需改成 FindAvailableIndex 等
        int chosenIndex = characterMaterialIndex[0];
        int indexValue = characterMaterialIndex[0];
        characterMaterialIndex.Remove(indexValue);

        // 仅在本地玩家加入时进行处理（与原逻辑保持一致）
        if (player == Runner.LocalPlayer)
        {
            // 如果本地池子里已经没有可用的Prefab，进行提示
            if (chosenIndex < 0 || chosenIndex >= PlayerPrefabsPool.Count)
            {
                Debug.LogWarning("No valid prefab index in the local pool.");
                return;
            }

            // 根据全局下标在本地池子中取出对应Prefab
            GameObject chosenPrefab = PlayerPrefabsPool[chosenIndex];
            PlayerPrefabsPool.RemoveAt(chosenIndex);

            // 使用该 Prefab 进行 Spawn
            NetworkObject plObject = Runner.Spawn(chosenPrefab, Vector3.zero, Quaternion.identity, player);
            plObject.name = "CurrentPlayer";

            // 记录该玩家使用的 Prefab 与下标
            assignedPrefabs[player] = chosenPrefab;
            assignedPrefabIndices[player] = chosenIndex;

            // 以下代码与原逻辑相同
            GameObject.Find("Inventory").GetComponent<Inventory>().dropPosition =
                plObject.transform.Find("DropBox").transform;
            GameObject.Find("WieldManager").GetComponent<WieldableManager>().wieldablesPosition =
                plObject.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            GameObject.Find("WieldManager").GetComponent<WieldableManager>().controller = plObject.GetComponent<PlayerController>();
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        // 如果玩家名下有PrefabIndex的记录，则将其归还至全局同步列表
        if (assignedPrefabIndices.TryGetValue(player, out int usedIndex))
        {
            characterMaterialIndex.Add(usedIndex);

            assignedPrefabIndices.Remove(player);
        }

        // 如果玩家名下有Prefab的引用，则将其归还至本地池子
        if (assignedPrefabs.TryGetValue(player, out var usedPrefab))
        {
            // 归还Prefab到池子
            PlayerPrefabsPool.Add(usedPrefab);
            assignedPrefabs.Remove(player);

            // Despawn该玩家的NetworkObject
            var netObj = Runner.GetPlayerObject(player);
            if (netObj != null)
            {
                Runner.Despawn(netObj);
            }
        }
        // 如果需要，可以在此处恢复原逻辑中对角色材质的释放等操作
    }
}
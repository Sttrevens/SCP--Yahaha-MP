using System.Collections;
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class RandomPrefabSpawner : NetworkBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("可以随机生成的Fusion Prefab数组")]
    public GameObject[] prefabArray; 

    [Tooltip("生成的位置偏移")] 
    public Vector3 spawnOffset = Vector3.zero;

    [Tooltip("延迟生成时间")]
    public float spawnDelay = 1f; // 延迟一秒生成

    public override void Spawned()
    {
        if (HasStateAuthority)
            StartCoroutine(SpawnPrefabWithDelay());
    }

    /// <summary>
    /// 延迟生成随机Prefab的协程
    /// </summary>
    private IEnumerator SpawnPrefabWithDelay()
    {
        // 延迟一段时间
        yield return new WaitForSeconds(spawnDelay);

        // 检查是否有Prefab可以生成
        if (prefabArray == null || prefabArray.Length == 0)
        {
            Debug.LogWarning("Prefab数组为空，无法生成对象！");
            yield break;
        }

        // 从数组中随机选择一个Prefab
        int randomIndex = Random.Range(0, prefabArray.Length);
        GameObject selectedPrefab = prefabArray[randomIndex];

        // 使用Fusion的网络实例化函数生成对象
        if (HasStateAuthority)
        {
            NetworkObject spawnedPrefab = Runner.Spawn(
                selectedPrefab,                  // 需要生成的Prefab
                transform.position + spawnOffset, // 生成位置（相对于当前对象）
                Quaternion.identity
            );

            if (spawnedPrefab.GetComponent<NavMeshAgent>() != null)
            {
                NavMeshAgent agent = spawnedPrefab.GetComponent<NavMeshAgent>();
                agent.enabled = false;
                spawnedPrefab.transform.position = transform.position + spawnOffset;
                agent.enabled = true;
            }
            else
            {
                spawnedPrefab.transform.position = transform.position + spawnOffset;
            }
        }
    }
}
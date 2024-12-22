using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnRadius = 10f;  // 刷怪范围半径
    public float spawnInterval = 2f; // 刷怪的时间间隔
    public List<SpawnableMonster> monsters; // 可生成的怪物类型和生成概率

    private void Start()
    {
        // 启动生成怪物的协程
        StartCoroutine(SpawnMonsters());
    }

    // 刷怪逻辑
    private IEnumerator SpawnMonsters()
    {
        while (true)
        {
            // 等待生成间隔
            yield return new WaitForSeconds(spawnInterval);

            // 随机选择一个怪物
            SpawnableMonster monsterToSpawn = GetRandomMonster();

            // 随机生成一个位置
            Vector3 spawnPosition = GetRandomPosition();

            // 实例化怪物
            if (monsterToSpawn != null && monsterToSpawn.monsterPrefab != null)
            {
                Instantiate(monsterToSpawn.monsterPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    // 随机选择一个怪物
    private SpawnableMonster GetRandomMonster()
    {
        // 按照概率选择怪物
        float totalWeight = 0f;
        foreach (var monster in monsters)
        {
            totalWeight += monster.spawnProbability;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (var monster in monsters)
        {
            if (randomValue < monster.spawnProbability)
            {
                return monster;
            }
            randomValue -= monster.spawnProbability;
        }

        return null;
    }

    // 获取一个范围内的随机位置
    private Vector3 GetRandomPosition()
    {
        // 在一个球形范围内随机生成位置
        Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
        randomPosition += transform.position;  // 相对于刷怪笼的位置

        return randomPosition;
    }

    // 定义怪物的类型和生成频率
    [System.Serializable]
    public class SpawnableMonster
    {
        public GameObject monsterPrefab; // 怪物Prefab
        public float spawnProbability = 1f; // 生成概率，可以控制每个怪物的生成频率
    }
}

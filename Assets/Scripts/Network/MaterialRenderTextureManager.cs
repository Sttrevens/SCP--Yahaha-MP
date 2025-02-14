using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;

public class MaterialRenderTextureManager : NetworkBehaviour
{
    // 预定义的四个材质和渲染纹理（保持不变）
    [SerializeField] private Material[] availableMaterials;
    [SerializeField] private RenderTexture[] availableRenderTextures;

    // 角色材质改为“材质列表的列表”，每个子列表包含3个材质
    public List<CharacterMaterialSet> availableCharacterMaterialSets;

    // 存储当前玩家分配的材质和渲染纹理（保持不变）
    private Dictionary<NetworkObject, (Material, RenderTexture)> playerAssignments;
    // 存储当前玩家分配的角色材质索引（引用某个Material[]的下标）
    private Dictionary<NetworkObject, int> characterAssignments;

    // 使用 NetworkLinkedList 代替原先的 List<T>（保持不变）
    [Networked, Capacity(4)]
    private NetworkLinkedList<int> assignedMaterialsIndexes { get; }

    [Networked, Capacity(4)]
    private NetworkLinkedList<int> assignedRenderTexturesIndexes { get; }

    [Networked, Capacity(4)]
    private NetworkLinkedList<int> assignedCharacterMaterialsIndexes { get; }

    public static MaterialRenderTextureManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 初始化玩家分配字典
            playerAssignments = new Dictionary<NetworkObject, (Material, RenderTexture)>();
            characterAssignments = new Dictionary<NetworkObject, int>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 该示例假设只在服务器(HasStateAuthority)上修改同步数据
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // 此处保留或根据需要添加逻辑
        }
    }

    // 玩家进入游戏时，分配材质和渲染纹理（保持不变）
    public void AssignMaterialAndRenderTexture(NetworkObject player)
    {
        if (!HasStateAuthority) 
            return;

        if (assignedMaterialsIndexes.Count < availableMaterials.Length &&
            assignedRenderTexturesIndexes.Count < availableRenderTextures.Length)
        {
            int availableMaterialIndex = FindAvailableIndex(assignedMaterialsIndexes, availableMaterials.Length);
            int availableRenderTextureIndex = FindAvailableIndex(assignedRenderTexturesIndexes, availableRenderTextures.Length);

            if (availableMaterialIndex >= 0 && availableRenderTextureIndex >= 0)
            {
                Material material = availableMaterials[availableMaterialIndex];
                RenderTexture renderTexture = availableRenderTextures[availableRenderTextureIndex];

                playerAssignments[player] = (material, renderTexture);

                assignedMaterialsIndexes.Add(availableMaterialIndex);
                assignedRenderTexturesIndexes.Add(availableRenderTextureIndex);

                ApplyMaterialAndRenderTexture(player, material, renderTexture);
            }
            else
            {
                Debug.LogWarning("No available material/render texture index found.");
            }
        }
        else
        {
            Debug.LogWarning("No available material/render texture for the player.");
        }
    }

    // 玩家退出时，释放资源（保持不变）
    public void ReleaseMaterialAndRenderTexture(NetworkObject player)
    {
        if (!HasStateAuthority)
            return;

        if (playerAssignments.TryGetValue(player, out (Material material, RenderTexture renderTexture) pair))
        {
            var (material, rt) = pair;

            int materialIndex = System.Array.IndexOf(availableMaterials, material);
            int renderTextureIndex = System.Array.IndexOf(availableRenderTextures, rt);

            assignedMaterialsIndexes.Remove(materialIndex);
            assignedRenderTexturesIndexes.Remove(renderTextureIndex);

            playerAssignments.Remove(player);

            ApplyMaterialAndRenderTexture(player, null, null);
        }
    }

    // 根据已分配的索引，查找可用的材质/纹理下标（保持不变）
    private int FindAvailableIndex(NetworkLinkedList<int> assignedIndexes, int maxCount)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (!assignedIndexes.Contains(i))
            {
                return i;
            }
        }
        return -1;  // 未找到可用索引
    }

    // 将材质和渲染纹理应用到玩家的相机道具（保持不变）
    private void ApplyMaterialAndRenderTexture(NetworkObject player, Material material, RenderTexture renderTexture)
    {
        var playerCameraProp = player.GetComponentInChildren<CameraController>(true);
        if (playerCameraProp != null)
        {
            playerCameraProp.SetMaterialAndRenderTexture(material, renderTexture);
        }
    }


    /// <summary>
    /// 分配角色材质（3个），并将它们应用到玩家对象下 Model/Male_01
    /// </summary>
    public void AssignCharacterMaterial(NetworkObject player)
    {
        //if (!HasStateAuthority) return;

        if (assignedCharacterMaterialsIndexes.Count < availableCharacterMaterialSets.Count)
        {
            int availableIndex = FindAvailableIndex(assignedCharacterMaterialsIndexes, availableCharacterMaterialSets.Count);

            if (availableIndex >= 0)
            {
                CharacterMaterialSet charMatSet = availableCharacterMaterialSets[availableIndex];
                characterAssignments[player] = availableIndex;

                assignedCharacterMaterialsIndexes.Add(availableIndex);

                player.GetComponent<PlayerData>().characterMaterialIndex = availableIndex;
            }
            else
            {
                Debug.LogWarning("No available character material set found.");
            }
        }
        else
        {
            Debug.LogWarning("No available character material set for the player.");
        }
    }

    /// <summary>
    /// 释放角色材质（3个）
    /// </summary>
    public void ReleaseCharacterMaterial(NetworkObject player)
    {
        if (!HasStateAuthority) return;

        if (characterAssignments.TryGetValue(player, out int charMatSetIndex))
        {
            assignedCharacterMaterialsIndexes.Remove(charMatSetIndex);
            characterAssignments.Remove(player);

            // // 清理角色材质
            // ApplyCharacterMaterial(player, null);
        }
    }

    // 给角色的 Model/Male_01 应用或清除对应的3个材质
    public void ApplyCharacterMaterial(NetworkObject player, CharacterMaterialSet charMatSet)
    {
        Transform male01 = player.transform.Find("Model/Male_01");
        if (male01 != null)
        {
            var renderer = male01.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (charMatSet != null)
                    renderer.materials = new Material[] { charMatSet.uvMat, charMatSet.lightMat, charMatSet.normalMat };
                // else
                //     renderer.materials = new Material[0];
            }
        }
    }
}

[System.Serializable]
  public class CharacterMaterialSet
  {
      public Material uvMat;
      public Material lightMat;
      public Material normalMat;
  }
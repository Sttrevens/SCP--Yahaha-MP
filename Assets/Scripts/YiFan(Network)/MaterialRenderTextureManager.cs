using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;

public class MaterialRenderTextureManager : NetworkBehaviour
{
    // 预定义的四个材质和渲染纹理
    [SerializeField] private Material[] availableMaterials;
    [SerializeField] private RenderTexture[] availableRenderTextures;

    // 存储当前玩家分配的材质和渲染纹理
    private Dictionary<NetworkObject, (Material, RenderTexture)> playerAssignments;

    // 使用 NetworkLinkedList 代替原先的 List<T>
    // Capacity(4) 仅供示例，可根据你能支持的最大玩家/材质数量调整
    [Networked, Capacity(4)]
    private NetworkLinkedList<int> assignedMaterialsIndexes { get; }

    [Networked, Capacity(4)]
    private NetworkLinkedList<int> assignedRenderTexturesIndexes { get; }

    public static MaterialRenderTextureManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 初始化玩家分配字典
            playerAssignments = new Dictionary<NetworkObject, (Material, RenderTexture)>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 该示例假设只在服务器(HasStateAuthority)上修改同步数据
    public override void FixedUpdateNetwork()
    {
        // 如果你需要在这里进行某些逻辑，可自行添加；否则可留空
        if (HasStateAuthority)
        {
            // ...
        }
    }

    // 玩家进入游戏时，分配材质和渲染纹理
    public void AssignMaterialAndRenderTexture(NetworkObject player)
    {
        // 如果本地没有 StateAuthority，就不执行分配逻辑
        if (!HasStateAuthority) 
            return;

        // 检查当前分配数量是否未超出数组大小
        if (assignedMaterialsIndexes.Count < availableMaterials.Length &&
            assignedRenderTexturesIndexes.Count < availableRenderTextures.Length)
        {
            // 找到可用索引
            int availableMaterialIndex = FindAvailableIndex(assignedMaterialsIndexes, availableMaterials.Length);
            int availableRenderTextureIndex = FindAvailableIndex(assignedRenderTexturesIndexes, availableRenderTextures.Length);

            // 如果存在可用的材质/纹理索引，则执行分配操作
            if (availableMaterialIndex >= 0 && availableRenderTextureIndex >= 0)
            {
                Material material = availableMaterials[availableMaterialIndex];
                RenderTexture renderTexture = availableRenderTextures[availableRenderTextureIndex];

                // 记下分配结果
                playerAssignments[player] = (material, renderTexture);

                // 在网络同步容器中记录该索引
                assignedMaterialsIndexes.Add(availableMaterialIndex);
                assignedRenderTexturesIndexes.Add(availableRenderTextureIndex);

                // 应用材质和渲染纹理
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

    // 玩家退出时，释放资源
    public void ReleaseMaterialAndRenderTexture(NetworkObject player)
    {
        // 如果本地没有 StateAuthority，就不执行释放逻辑
        if (!HasStateAuthority)
            return;

        if (playerAssignments.TryGetValue(player, out (Material material, RenderTexture renderTexture) pair))
        {
            var (material, rt) = pair;

            // 找到对应的材质和渲染纹理索引
            int materialIndex = System.Array.IndexOf(availableMaterials, material);
            int renderTextureIndex = System.Array.IndexOf(availableRenderTextures, rt);

            // 从网络同步容器中移除对应索引
            assignedMaterialsIndexes.Remove(materialIndex);
            assignedRenderTexturesIndexes.Remove(renderTextureIndex);

            // 清除玩家分配记录
            playerAssignments.Remove(player);

            // 清除材质和渲染纹理
            ApplyMaterialAndRenderTexture(player, null, null);
        }
    }

    // 根据已分配的索引，查找可用的材质/纹理下标
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

    // 将材质和渲染纹理应用到玩家的相机道具
    private void ApplyMaterialAndRenderTexture(NetworkObject player, Material material, RenderTexture renderTexture)
    {
        // 假设玩家的相机道具有 `CameraController` 组件
        var playerCameraProp = player.GetComponentInChildren<CameraController>(true);
        if (playerCameraProp != null)
        {
            playerCameraProp.SetMaterialAndRenderTexture(material, renderTexture);
        }
    }
}
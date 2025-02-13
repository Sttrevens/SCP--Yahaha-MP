using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    [Networked]
    public string PlayerName { get; set; }

    [Networked] public int characterMaterialIndex { get; set; }

    private void Start()
    {
        if (HasStateAuthority && TitleScreenUI.playerName != null)
        {
            // 如果是这个玩家自己，可以设置名字（例如通过输入框、UI 等方式）
            PlayerName = TitleScreenUI.playerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Render()
    {
        CharacterMaterialSet charMatSet = MaterialRenderTextureManager.Instance.availableCharacterMaterialSets[characterMaterialIndex];
        MaterialRenderTextureManager.Instance.ApplyCharacterMaterial(Object, charMatSet);
    }
}

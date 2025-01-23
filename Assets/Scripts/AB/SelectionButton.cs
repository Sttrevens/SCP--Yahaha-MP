using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class SelectionButton : NetworkBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private int index; 
    [SerializeField] private Color selectedColor = Color.green; 
    [SerializeField] private Color defaultColor = Color.white; 

    private void Start()
    {
        UpdateButtonAppearance();
        button.onClick.AddListener(() => RPC_ToggleSelection(index));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ToggleSelection(int index)
    {
        AudioManager.Instance.PlayStartButtonSound();
        bool isSelected = LevelManager.Instance.roomIndexSelected == index;
        LevelManager.Instance.roomIndexSelected = isSelected ? -1 : index; 
        LevelManager.Instance.isButtonSelected = !isSelected; 
        LevelManager.Instance.RPC_UpdateAllButtons();
    }

    public void UpdateButtonAppearance()
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = LevelManager.Instance.roomIndexSelected == index ? selectedColor : defaultColor;
        button.colors = colorBlock;
    }
}

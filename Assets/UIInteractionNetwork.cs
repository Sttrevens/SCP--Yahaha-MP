using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class UIInteractionNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private GameObject levelPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ClickShopButton(bool isShop)
    {
        AudioManager.Instance.PlayStartButtonSound();
        if (isShop)
        {
            shopPanel.SetActive(true);
            levelPanel.SetActive(false);
        }
        else
        {
            levelPanel.SetActive(true);
            shopPanel.SetActive(false);
        }
    }
}

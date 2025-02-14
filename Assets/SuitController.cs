using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SuitController : NetworkBehaviour, IInteractable
{
    [SerializeField] private int suitIndex;

    public string GetInteractText()
    {
        return string.Format("{0}", "Wear the suit");
    }

    public void OnInteract()
    {
        if(HasStateAuthority)
        {
            GameObject.Find("CurrentPlayer").GetComponent<PlayerData>().characterMaterialIndex = suitIndex;
            RPC_OnInteract();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnInteract()
    {
        //Destroy(gameObject);
    }
}

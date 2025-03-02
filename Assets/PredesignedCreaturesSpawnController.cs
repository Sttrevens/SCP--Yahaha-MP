using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PredesignedCreaturesSpawnController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SpawnCreatures()
    {
        StartCoroutine(SpawnCreatures());
    }

    IEnumerator SpawnCreatures()
    {
        yield return new WaitForSeconds(1f);

        Transform[] childObjects = transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in childObjects)
        {
            if (child != transform)
                child.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class CreaturesSpawnController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
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

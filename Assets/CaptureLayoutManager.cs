using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq; // 添加这行

public class CaptureLayoutManager : NetworkBehaviour
{
    [SerializeField] private GameObject spPanel; // Single player panel
    [SerializeField] private GameObject mpPanel; // Multiplayer panel
    
    private NetworkRunner _runner;

    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
        if (_runner == null)
        {
            Debug.LogError("NetworkRunner not found!");
            return;
        }

        if (spPanel != null)
            spPanel.SetActive(false);
            
        if (mpPanel != null)
            mpPanel.SetActive(false);
    }

    private void Update()
    {
        if (_runner == null) return;

        int playerCount = _runner.ActivePlayers.Count(); // 使用 Count() 方法
        
        if (spPanel != null)
            spPanel.SetActive(playerCount == 1);
            
        if (mpPanel != null)
            mpPanel.SetActive(playerCount > 1);
    }
}
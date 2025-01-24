using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class SpaceshipMonitorController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text playerNamesText;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text roomNameText;

    private ScoreManager scoreManager;
    private FusionBootstrap bootstrap;

    void Start()
    {
        // Find required components
        scoreManager = FindObjectOfType<ScoreManager>();

        // Initialize UI texts
        if (playerNamesText == null || totalScoreText == null || roomNameText == null)
        {
            Debug.LogError("UI Text references not set in SpaceshipMonitorController");
        }
    }

    void Update()
    {
        UpdatePlayerNames();
        UpdateTotalScore();
        UpdateRoomName();
    }

    private void UpdatePlayerNames()
    {
        if (playerNamesText == null) return;

        PlayerData[] players = FindObjectsOfType<PlayerData>();
        string names = "";

        foreach (PlayerData player in players)
        {
            names += player.PlayerName + "\n";
        }

        playerNamesText.text = names;
    }

    private void UpdateTotalScore()
    {
        if (totalScoreText == null || scoreManager == null) return;

        totalScoreText.text = "Total Viewers: " + scoreManager.totalScore.ToString("F0") + "\n" + "Total Income: " + "\n" + (scoreManager.totalScore / 20).ToString("F2") + " $";
    }

    private void UpdateRoomName()
    {
        if (roomNameText == null || bootstrap == null) return;

bootstrap = FindObjectOfType<FusionBootstrap>();
        roomNameText.text = "Current Room Name: " + "\n" + bootstrap.DefaultRoomName;
    }
}

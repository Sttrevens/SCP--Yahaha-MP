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
    [SerializeField] private TMP_Text dayText;

    private ScoreManager scoreManager;
    private NetworkStart networkStart;
    private ControlSticksController controlSticksController;

    void Start()
    {
        // Find required components
        scoreManager = FindObjectOfType<ScoreManager>();
        networkStart = FindObjectOfType<NetworkStart>();
        controlSticksController = FindObjectOfType<ControlSticksController>();

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
        if (playerNamesText == null)
        {
            Debug.LogWarning("PlayerNamesText is null in SpaceshipMonitorController");
            return;
        }

        PlayerData[] players = FindObjectsOfType<PlayerData>();
        //Debug.Log($"Found {players.Length} players in the scene");

        string names = "";

        foreach (PlayerData player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("Found null PlayerData object");
                continue;
            }

            if (string.IsNullOrEmpty(player.PlayerName))
            {
                Debug.LogWarning("Player has empty or null name");
                names += "Unknown Player\n";
            }
            else
            {
                names += player.PlayerName + "\n";
            }
        }

        //Debug.Log($"Updating player names text with: {names}");
        playerNamesText.text = names;
    }

    private void UpdateTotalScore()
    {
        if (totalScoreText == null || scoreManager == null || controlSticksController == null) return;

        totalScoreText.text = "Total Viewers: " + scoreManager.networkedTotalScore + "\n" + "Total Revenue: " + "\n" + scoreManager.revenueRate + "/1000 $";
        if (controlSticksController.currentDays <= 3)
        {
            dayText.text = "Day: " + controlSticksController.currentDays.ToString()  + "/3";
        }
        else if (controlSticksController.currentDays > 3 && scoreManager.networkedTotalScore >= 1000)
        {
            dayText.text = "Day: " + controlSticksController.currentDays.ToString() + "/3" + "\n" + "You reached the target revenue!";
        }
        else if (controlSticksController.currentDays > 3 && scoreManager.networkedTotalScore < 1000)
        {
            dayText.text = "Day: " + controlSticksController.currentDays.ToString() + "/3" + "\n" + "You didn't reach the target revenue! Die in peace in space.";
        }
    }

    private void UpdateRoomName()
    {
        if (roomNameText == null)
        {
            Debug.LogWarning("roomNameText is null in SpaceshipMonitorController");
            return;
        }

        if (networkStart == null)
        {
            Debug.Log("networkStart is null, searching for NetworkStart instance");
            networkStart = FindObjectOfType<NetworkStart>();
        }

        if (networkStart == null)
        {
            Debug.LogError("Failed to find NetworkStart instance");
            roomNameText.text = "Current Room Name: \nNot Available";
            return;
        }

        if (string.IsNullOrEmpty(networkStart.roomName))
        {
            Debug.LogWarning("roomName is null or empty");
            roomNameText.text = "Current Room Name: \nUnnamed Room";
        }
        else
        {
            //Debug.Log($"Updating room name with: {networkStart.roomName}");
            roomNameText.text = "Current Room Name: \n" + networkStart.roomName;
        }
    }
}

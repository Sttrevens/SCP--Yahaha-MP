using System.Collections.Generic;
using LPSurvivalEngine;
using UnityEngine;

public class SettleController : MonoBehaviour
{
    [SerializeField] private List<BobojianZhuBaoTracker> bobojianZhuBaoTrackers;
    [SerializeField] private GameObject inputField;
    
    private enum PlayerStatus
    {
        Dead,
        Alive,
        Missing
    }
    
    private void Update()
    {
        HideAllStatusTexts();
        
        if (ControlSticksController.Instance.isSettling)
        {
            UpdatePlayerStatuses();
            inputField.SetActive(false);
        }
        else
        {
            HideAllStatusTexts();
            inputField.SetActive(true);
        }
    }
    
    private void UpdatePlayerStatuses()
    {
        foreach (var tracker in bobojianZhuBaoTrackers)
        {
            if (tracker.playerData == null)
            {
                continue;
            }
            
            PlayerStatus status = DeterminePlayerStatus(tracker);
            DisplayPlayerStatus(tracker, status);
        }
    }
    
    private PlayerStatus DeterminePlayerStatus(BobojianZhuBaoTracker tracker)
    {
        HealthSystem healthSystem = tracker.playerData.GetComponent<HealthSystem>();
        
        if (healthSystem.isDeadNetworked)
        {
            return PlayerStatus.Dead;
        }
        else if (healthSystem.isInSpaceShip)
        {
            return PlayerStatus.Alive;
        }
        else
        {
            return PlayerStatus.Missing;
        }
    }
    
    private void DisplayPlayerStatus(BobojianZhuBaoTracker tracker, PlayerStatus status)
    {
        tracker.settledStatusText.gameObject.SetActive(true);
        tracker.noSignalText.gameObject.SetActive(false);
        
        switch (status)
        {
            case PlayerStatus.Dead:
                tracker.settledStatusText.text = "DEAD";
                break;
            case PlayerStatus.Alive:
                tracker.settledStatusText.text = "ALIVE";
                break;
            case PlayerStatus.Missing:
                tracker.settledStatusText.text = "MISSING";
                break;
        }
    }
    
    private void HideAllStatusTexts()
    {
        foreach (var tracker in bobojianZhuBaoTrackers)
        {
            tracker.settledStatusText.gameObject.SetActive(false);
        }
    }
}
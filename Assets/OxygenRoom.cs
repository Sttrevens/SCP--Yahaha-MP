using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

public class OxygenRoom : MonoBehaviour
{
    [SerializeField] private ControlSticksController controlSticksController;
    private void OnTriggerStay(Collider other)
    {
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            Debug.Log($"Setting isInOxygenRoom to true for {other.name}");
            healthSystem.isInSpaceShip = true;
        }
        else
        {
            Debug.Log($"No HealthSystem component found on {other.name}");
        }
        
        if (controlSticksController != null && 
            controlSticksController.CurrentState == ControlSticksController.SpaceshipState.PreparingForTakeoff)
        {
            Debug.Log("Spaceship is in PreparingForTakeoff state");
            if (healthSystem != null)
            {
                Debug.Log($"Setting isInOxygenRoom to true for {other.name}");
                healthSystem.isInOxygenRoom = true;
            }
            else
            {
                Debug.Log($"No HealthSystem component found on {other.name}");
            }
        }
        else
        {
            Debug.Log("Spaceship is NOT in PreparingForTakeoff state");
            if (healthSystem != null)
            {
                Debug.Log($"Setting isInOxygenRoom to false for {other.name}");
                healthSystem.isInOxygenRoom = false;
            }
            else
            {
                Debug.Log($"No HealthSystem component found on {other.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit called with {other.name}");
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.isInOxygenRoom = false;
            healthSystem.isInSpaceShip = false;
        }
    }
}

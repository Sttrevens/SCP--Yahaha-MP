using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

public class OxygenRoom : MonoBehaviour
{
    [SerializeField] private ControlSticksController controlSticksController;
    private void OnTriggerStay(Collider other)
    {
        if (controlSticksController != null && 
            controlSticksController.CurrentState == ControlSticksController.SpaceshipState.PreparingForTakeoff)
        {
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.isInOxygenRoom = true;
            }
        }
        else
        {
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.isInOxygenRoom = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.isInOxygenRoom = false;
        }
    }
}

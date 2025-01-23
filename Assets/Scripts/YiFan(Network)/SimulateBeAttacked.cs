using LPSurvivalEngine;
using UnityEngine;

public class SimulateBeAttacked : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<HealthSystem>().TakePhysicDamage(12);
        }
    }
}

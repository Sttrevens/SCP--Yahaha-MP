using UnityEngine;

public class SimulateAttack : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<EnemyAI>().TakeDamage(10f);
        }
    }
}

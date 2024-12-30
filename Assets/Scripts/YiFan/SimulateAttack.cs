using UnityEngine;

public class SimulateAttack : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<EnemyAI>().DealDamgeRpc(10f);
        }
    }
}

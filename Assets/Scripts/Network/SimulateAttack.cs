using UnityEngine;

public class SimulateAttack : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<Enemy>().TakeDamage(10f);
        }
    }
}

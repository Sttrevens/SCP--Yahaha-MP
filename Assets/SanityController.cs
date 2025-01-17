using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;

public class SanityController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<ConeDetection>().realtimeScore > 0)
        {
            GameObject.Find("CurrentPlayer").GetComponent<HealthSystem>().sanity.Subtract(GetComponent<ConeDetection>().realtimeScore * Time.fixedDeltaTime);
        }
    }
}

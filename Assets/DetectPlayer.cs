using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public bool detectPlayerResult = false;
    public Transform player;
    
    // Start is called before the first frame update
    void Start()
    {
        detectPlayerResult = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayerResult = true;
            player = other.transform;
        }
        
        Debug.Log("Spot lighting: " + other.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayerResult = false;
            player = null;
        }
    }
}

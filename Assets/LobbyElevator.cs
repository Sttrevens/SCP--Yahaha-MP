using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyElevator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AnimationTrigger>().TriggerAnimatoin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

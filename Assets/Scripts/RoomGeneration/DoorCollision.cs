using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollision : MonoBehaviour
{

    public bool isEntrance;
    private bool generated = false;

    void OnTriggerEnter(Collider other)
    {
        print("entered");
        if (other.CompareTag("Player")) 
        {
            if (isEntrance)
            {
                RoomGeneration.Instance.point = 0;
            }
            else
            {
                RoomGeneration.Instance.point += 1;
            }

            if (!generated)
            {
                RoomGeneration.Instance.GenerateNewRoom();
                generated = true;
            }
            
            
        }
    }
}

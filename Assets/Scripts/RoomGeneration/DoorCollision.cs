using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    public bool isEntrance;
    public bool entered = false;
    public GameObject Room;
    private bool isFinal = false;

    void OnTriggerEnter(Collider other)
    {
        if (!entered)
        {
            RoomGeneration.Instance.DeleteLastRoom();
            if (other.CompareTag("Player"))
            {
                entered = true;
                Room.GetComponent<NewRoom>().AdjustRoom();
            } 
        }
        else if(!isFinal)
        {
            isFinal = true;
            if (isEntrance)
            {
                RoomGeneration.Instance.point = 0;
                Room.GetComponent<NewRoom>().failed = true;
            }
            else
            {
                RoomGeneration.Instance.point += 1;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDoor : MonoBehaviour
{
    public bool generated = false;
    public bool shouldGenerate;
    [SerializeField] private Transform newRoomPosition;
    [SerializeField] private Transform oldRoomPosition;

    void OnTriggerEnter(Collider other)
    {     
        if (other.CompareTag("Player"))
        {

            if (!generated && shouldGenerate)
            {
                RoomGeneration.Instance.DeleteLastCorridor();
                RoomGeneration.Instance.GenerateNewRoom(newRoomPosition);
                //RoomGeneration.Instance.GenerateNewRoom(oldRoomPosition);
                generated = true;
            }

        }
    }
}

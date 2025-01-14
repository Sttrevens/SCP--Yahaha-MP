using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDoor : MonoBehaviour
{
    public bool generated = false;
    public bool shouldGenerate;
    public bool isEntrance;
    [SerializeField] private Transform newRoomPosition;
    [SerializeField] private Transform oldRoomPosition;

    void OnTriggerEnter(Collider other)
    {     
        if (other.CompareTag("Player"))
        {
            if (!generated && shouldGenerate)
            {
                if (isEntrance)
                {
                    RoomGeneration.Instance.GenerateNewRoom(newRoomPosition);
                }else
                {
                    RoomGeneration.Instance.DeleteLastRoom();
                }
                generated = true;
            }

        }
    }

    public void RevertDirection()
    {
        isEntrance = !isEntrance;
        Vector3 newPos = newRoomPosition.position;
        newRoomPosition.position = oldRoomPosition.position;
        oldRoomPosition.position = newPos;
    }
    
}

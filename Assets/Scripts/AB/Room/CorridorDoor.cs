using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDoor : MonoBehaviour
{
    public bool generated = false;
    public bool shouldGenerate;
    private bool hasReentered = false;
    public bool isEntrance;
    [SerializeField] private Transform newRoomPosition;
    [SerializeField] private Transform oldRoomPosition;

    void OnTriggerEnter(Collider other)
    {     
        if (other.CompareTag("Player"))
        {

            if (shouldGenerate)
            {
                //first time enter
                if (isEntrance)
                {
                    if (!generated)
                    {
                        RoomGeneration.Instance.GenerateNewRoom(newRoomPosition);
                        generated = true;
                    }

                }
                else
                {
                    //first time leave exit
                    hasReentered = true;
                    RoomGeneration.Instance.DeleteLastRoom(); 
                }
                if(hasReentered)//reenter
                {
                    RoomGeneration.Instance.ResetLastCorridor();
                    RoomGeneration.Instance.GenerateNewRoom(oldRoomPosition);
                }
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

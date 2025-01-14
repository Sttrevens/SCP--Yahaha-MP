using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    public bool isEntrance;
    public bool entered = false;
    public GameObject Room;
    private bool isFinal = false;
    [SerializeField] private Transform newPosition;
    [SerializeField] private Transform oldPosition;
    

    void OnTriggerEnter(Collider other)
    {
        if (!entered)
        {
            if (other.CompareTag("Player"))
            {
                RoomGeneration.Instance.GenerateNewCorridor(newPosition);
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
                //how about when room is reverted which corridor should i delete?
                RoomGeneration.Instance.DeleteLastCorridor();
                RoomGeneration.Instance.point += 1;
            }
        }

    }
}

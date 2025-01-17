using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoom : MonoBehaviour
{
    public bool isWrong = false;
    public int roomIndex = 0;
    public GameObject entrancePosition;
    public GameObject exitPosition;

    public Transform exitGeneratedPosition;
    public Transform entranceGeneratedPosition;

    public GameObject last;
    public bool failed;
    
    void Start()
    {
        isWrong = false;
        failed = false;
    }

    public void MakeRoom(int index)
    {
        roomIndex = index;
        isWrong = false;
    }

    public void AdjustRoom()
    {
        if (roomIndex != 0)
        {
            isWrong = true;
            //swap
            Transform tempPosition = exitPosition.transform;
            exitPosition.transform.position = entrancePosition.transform.position;
            entrancePosition.transform.position = tempPosition.position;
        }
        entrancePosition.GetComponent<DoorCollision>().isEntrance = true;
        exitPosition.GetComponent<DoorCollision>().isEntrance = false;
    }
}

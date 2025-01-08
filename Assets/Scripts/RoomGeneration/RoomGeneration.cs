using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class RoomGeneration : MonoBehaviour
{
    private static RoomGeneration instance;

    public static RoomGeneration Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomGeneration>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(RoomGeneration).ToString());
                    instance = singletonObject.AddComponent<RoomGeneration>();
                }
            }
            return instance;
        }
    }

    public float point;
    
    public GameObject roomPrefab;
    [SerializeField] private GameObject[] rooms;
    
    private Vector3 spawnPosition;

    // Keeps track of the last room's position
    private GameObject lastRoom;
    private GameObject currentRoom;
    [SerializeField]private GameObject firstRoom;
    
    public bool isDoorActive;
    public bool isRoomChanging;
    
    void Start()
    {
        spawnPosition = Vector3.zero; 
        GenerateInitialRoom();
        point = 0;
    }
    
    
    private void GenerateInitialRoom()
    {
        lastRoom = firstRoom;
        isDoorActive = true;
    }
    
    public void GenerateNewRoom(bool isEntrance)
    {
        
        bool isRoomWrong = GetRandomBool();
        int roomIndex = 0;
        if (isRoomWrong)
        {
            roomIndex = GetRandomNumberInRange();
        }
        if (lastRoom.GetComponent<NewRoom>().isWrong)
        {
            spawnPosition = lastRoom.GetComponent<NewRoom>().entranceGeneratedPosition.position;
        }
        else
        {
            spawnPosition = lastRoom.GetComponent<NewRoom>().exitGeneratedPosition.position;
        }
        //if its an entrance make the exit active
        if(isEntrance)
        {
            if(currentRoom!=null) currentRoom.GetComponent<NewRoom>().AdjustRoom();
            isDoorActive = true;
        }
        //if it's an exit after walk past an entrance then generate the next room
        if (!isEntrance && isDoorActive)
        {
            GameObject newRoom = Instantiate(roomPrefab, lastRoom.GetComponent<NewRoom>().exitGeneratedPosition.position, Quaternion.identity);
            newRoom.GetComponent<NewRoom>().last = lastRoom;
            newRoom.GetComponent<NewRoom>().MakeRoom(roomIndex);
            isDoorActive = !isDoorActive;
            isRoomChanging = true;
        }
        //if the player walks entrance/exit twice
        else if(isRoomChanging)
        {
            currentRoom = Instantiate(roomPrefab, lastRoom.transform.position, Quaternion.identity);
            currentRoom.GetComponent<NewRoom>().MakeRoom(roomIndex);
            Destroy(lastRoom);
            currentRoom.GetComponent<NewRoom>().last = lastRoom;
            isRoomChanging = false;
        }   
    }
    
    public bool GetRandomBool()
    {
        return UnityEngine.Random.value > 0.5f;
    }
    
    public int GetRandomNumberInRange()
    {
        return UnityEngine.Random.Range(0, rooms.Length);
    }
}

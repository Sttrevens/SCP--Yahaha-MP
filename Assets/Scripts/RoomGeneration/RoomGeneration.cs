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
    private Transform lastRoom;
    [SerializeField]private Transform firstRoom;
    
    void Start()
    {
        spawnPosition = Vector3.zero; 
        GenerateInitialRoom();
        point = 0;
    }
    
    
    private void GenerateInitialRoom()
    {
        lastRoom = firstRoom.transform; 
    }
    
    public void GenerateNewRoom()
    {
        if (lastRoom.GetComponent<NewRoom>().isWrong)
        {
            spawnPosition = lastRoom.GetComponent<NewRoom>().entranceGeneratedPosition.position;
        }
        else
        {
            spawnPosition = lastRoom.GetComponent<NewRoom>().exitGeneratedPosition.position;
        }
        
        GameObject newRoom = Instantiate(roomPrefab, spawnPosition, Quaternion.identity);
        //newRoom.GetComponent<NewRoom>().last = lastRoom.gameObject;
        //Destroy(lastRoom.GetComponent<NewRoom>().last);
        GameObject _room = lastRoom.gameObject;
        lastRoom = newRoom.transform;
        //Destroy(_room);

        
        
       

        bool isRoomWrong = GetRandomBool();
        int roomIndex = 0;
        if (isRoomWrong)
        {
            roomIndex = GetRandomNumberInRange();
        }
        newRoom.GetComponent<NewRoom>().MakeRoom(roomIndex);
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

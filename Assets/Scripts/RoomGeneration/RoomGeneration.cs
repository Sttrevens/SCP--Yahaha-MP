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
    public GameObject corridorPrefab;
    [SerializeField] private GameObject[] rooms;
    

    // Keeps track of the last room's position
    private GameObject lastRoom;
    private GameObject lastCorridor;
    private GameObject newRoom;
    private GameObject newCorridor;
    [SerializeField]private GameObject entrancePoint;
    
    void Start()
    {
        point = 0;
    }
    
    private void GenerateInitialRoom()
    {
        lastCorridor =  entrancePoint;
    }
    
    public void GenerateNewRoom(Transform position)
    {
        //decide which room
        bool isRoomWrong = GetRandomBool();
        int roomIndex = 0;
        if (isRoomWrong)
        {
            roomIndex = GetRandomNumberInRange();
        }
        
        //generate room and corridor
        newRoom = Instantiate(roomPrefab, position.position, Quaternion.identity);
        newRoom.GetComponent<NewRoom>().last = lastRoom;
        newRoom.GetComponent<NewRoom>().MakeRoom(roomIndex);
    }
    
    public void GenerateNewCorridor(Transform position)
    {
        newCorridor = Instantiate(corridorPrefab, newRoom.GetComponent<NewRoom>().exitGeneratedPosition.position, Quaternion.identity);
    }

    public void DeleteLastRoom()
    {
        if(lastRoom!=null)Destroy(lastRoom);
        lastRoom = newRoom;

    }
       
    public void DeleteLastCorridor()
    {
        if(lastCorridor)Destroy(lastCorridor);
        lastCorridor = newCorridor;
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

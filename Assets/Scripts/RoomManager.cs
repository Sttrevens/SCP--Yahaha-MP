using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public GameObject initialRoomPrefab; // The initial fixed room where the player spawns
    public GameObject[] roomPrefabs; // Array of possible room prefabs to choose from
    public int maxGenerationSteps = 2; // Maximum steps to generate new rooms

    private int generationSteps = 0; // Tracks how many generations have been made
    private GameObject currentRoom; // The current room the player is in
    private Door[] currentRoomDoors; // Doors in the current room

    private void Start()
    {
        GenerateInitialRoom(); // Generate the first room on start
    }

    // Generates the initial room and its linked door
    void GenerateInitialRoom()
    {
        // Instantiate the initial room at the starting point
        currentRoom = Instantiate(initialRoomPrefab, Vector3.zero, Quaternion.identity);

        // Get the doors of the current room
        currentRoomDoors = currentRoom.GetComponentsInChildren<Door>();

        // Call to generate rooms connected to this initial room
        GenerateRoomsForCurrentRoom();
    }

    // Generate the next rooms for the current room
    void GenerateRoomsForCurrentRoom()
    {
        // Make sure we don't exceed the max generation steps
        if (generationSteps >= maxGenerationSteps)
        {
            return;
        }

        generationSteps++;

        foreach (var door in currentRoomDoors)
        {
            if (!door.HasConnectedRoom)
            {
                // Choose a random room prefab and instantiate it at the door's location
                GameObject newRoom = InstantiateRandomRoom(door.transform.position, door.transform.rotation);

                // Link the doors of the current room and the new room
                Door[] newRoomDoors = newRoom.GetComponentsInChildren<Door>();
                LinkDoors(door, newRoomDoors[0], newRoom);

                // Recursively generate rooms for the new room
                currentRoom = newRoom; // The current room now becomes the new room
                currentRoomDoors = newRoomDoors; // Get the new room's doors

                // Generate rooms connected to the new room
                GenerateRoomsForCurrentRoom();
                return;
            }
        }
    }

    // Instantiate a random room prefab
    GameObject InstantiateRandomRoom(Vector3 position, Quaternion rotation)
    {
        int randomIndex = Random.Range(0, roomPrefabs.Length);
        GameObject room = Instantiate(roomPrefabs[randomIndex], position, rotation);
        return room;
    }

    // Links the door from the current room to a door from the new room and ensures proper room alignment
    void LinkDoors(Door currentRoomDoor, Door newRoomDoor, GameObject newRoom)
    {
        currentRoomDoor.HasConnectedRoom = true;
        newRoomDoor.HasConnectedRoom = true;

        // Ensure the new room is positioned correctly relative to the current room's door
        Vector3 currentRoomDoorPosition = currentRoomDoor.transform.position;
        Vector3 newRoomDoorPosition = newRoomDoor.transform.position;

        // Calculate the difference in position to align the doors correctly
        Vector3 offset = currentRoomDoorPosition - newRoomDoorPosition;

        // Position the new room relative to the current room's door position
        newRoom.transform.position = currentRoom.transform.position + offset;

        // Optionally, adjust the rotation of the new room to ensure the doors align properly
        newRoom.transform.rotation = Quaternion.Euler(currentRoomDoor.transform.eulerAngles);
    }
}
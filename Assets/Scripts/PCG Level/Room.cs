using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] doors; // 房间的门
    public GameObject[] doorMarkers; // 对应门的位置和方向
    public bool[] isDoorConnected; // 标记门是否已经连接

    public void SetupRoom(int doorCount)
    {
        // 假设每个房间都有2-3个门，可以动态设置doorCount
        doors = new GameObject[doorCount];
        doorMarkers = new GameObject[doorCount];
        isDoorConnected = new bool[doorCount];

        // 初始化门和标记（这里假设门和标记已经预制好）
        for (int i = 0; i < doorCount; i++)
        {
            isDoorConnected[i] = false; // 初始状态下门是没有连接的
        }
    }

    public void SetDoorConnection(int doorIndex, bool isConnected)
    {
        isDoorConnected[doorIndex] = isConnected;
    }

    public void SetDoorMarker(int doorIndex, GameObject marker)
    {
        doorMarkers[doorIndex] = marker;
    }
}

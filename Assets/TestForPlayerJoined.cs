using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TestForPlayerJoined : SimulationBehaviour,IPlayerJoined
{
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("wuhuhuhuhuhuhuhu");
    }
}

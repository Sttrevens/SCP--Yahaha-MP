using Fusion;
using LPSurvivalEngine;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public Transform spawnPoint;
    
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject plObject = Runner.Spawn(PlayerPrefab, Vector3.zero, Quaternion.identity);
            plObject.name = "CurrentPlayer";
            Runner.SetPlayerObject(player, plObject);
            GameObject.Find("Inventory").GetComponent<Inventory>().dropPosition =
                plObject.transform.Find("DropBox").transform;
            GameObject.Find("WieldManager").GetComponent<WieldableManager>().wieldablesPosition =
                plObject.transform.Find("Model/Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR");
            // GameObject.Find("WieldManager").GetComponent<WieldableManager>().wieldablesPosition =
            //     plObject.transform.Find("jointItemR");
            GameObject.Find("WieldManager").GetComponent<WieldableManager>().controller = plObject.GetComponent<PlayerController>();
        }
    }
}
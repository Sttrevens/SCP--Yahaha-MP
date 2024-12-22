using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class WatchController : MonoBehaviour, IInteractable
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        /*GameObject[] uiPlayers = GameObject.FindGameObjectsWithTag("UI Player");
        if (uiPlayers.Length > 0)
        {
            foreach (GameObject uiPlayer in uiPlayers)
            {
                uiPlayer.SetActive(false);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetInteractText()
    {
        return string.Format("{0}", "High-tech Watch");
    }

    public void OnInteract()
    {
        /*GameObject[] uiPlayerObjects = player.GetComponentsInChildren<GameObject>(true).Where(g => g.CompareTag("UI Player")).ToArray();
        foreach (GameObject uiPlayerObject in uiPlayerObjects)
        {
            if (uiPlayerObject != null)
            {
                uiPlayerObject.SetActive(true);
            }
        }*/
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }    
}

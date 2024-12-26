using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyElevator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(gameStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator gameStart()
    {
        yield return new WaitForSeconds(3f);
        GetComponent<AnimationTrigger>().TriggerAnimatoin();
    }
}

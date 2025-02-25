using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCD : MonoBehaviour
{
    public Enemy enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        enemy.SwitchState(new WaitingforNextAttackState());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnterFurious : MonoBehaviour
{
    public ChasingEnemy chasingEnemy;
    public Enemy enemy;
    public JudaEyedSlimeRedController judaEyedSlimeRedController;
    
    public void GetHit()
    {
        chasingEnemy.targetPlayer = judaEyedSlimeRedController.target;
        chasingEnemy.detectionRange = judaEyedSlimeRedController.originalDetectionRange;
        Debug.Log("Get hit target: " + chasingEnemy.targetPlayer);
        enemy.SwitchState(new ChasingState());
    }
}

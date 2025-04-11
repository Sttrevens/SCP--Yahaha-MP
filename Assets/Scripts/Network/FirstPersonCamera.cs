using System;
using UnityEngine;
using UnityEngine.InputSystem; // 新 Input System 命名空间
using UnityEngine.InputSystem.LowLevel; 
using LPSurvivalEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float Height = 0.7f;
    public float MouseSensitivity = 10f;

    private void Update()
    {
        if (Target == null) return;
        
        transform.SetParent(Target);
        transform.localPosition = new Vector3(0, Height, 0);
    }
}
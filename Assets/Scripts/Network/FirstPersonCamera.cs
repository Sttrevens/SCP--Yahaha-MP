using System;
using UnityEngine;
using UnityEngine.Rendering;

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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


public class UpdateAimTarget : MonoBehaviour
{
    public Transform aimTarget;


    private void LateUpdate()
    {
        aimTarget.position = transform.position;
        aimTarget.rotation = transform.rotation;
    }
}

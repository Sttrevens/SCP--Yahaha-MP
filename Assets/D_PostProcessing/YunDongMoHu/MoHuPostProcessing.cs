using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("custom/MoHuPostProcessing")]
public class MoHuPostProcessing : VolumeComponent
{
    public ClampedFloatParameter distortionStrength = new ClampedFloatParameter(0.1f, 0, 1, true);
    public ClampedFloatParameter distortionSpeed = new ClampedFloatParameter(0.1f, 0, 1, true);
}
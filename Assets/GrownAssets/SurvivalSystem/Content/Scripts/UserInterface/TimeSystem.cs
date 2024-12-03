using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
   public class TimeSystem : MonoBehaviour
   {
    [Space]
    [Header("Time System")]
    [Space]
    [Space]

    [Space]
    [Header("Time Settings")]
    [Space]

    [Range(0.0f,1.0f)]
    public float time ;
    public float fulldayLength;
    public float startTime = 0.5f;
    private float timeRate;
    public Vector3 noon;

    [Space]
    [Header("Sun")] 
    [Space]

    public Light sun;

    [Space]

    public Gradient sunColor;
    
    [Space]

    public AnimationCurve sunIntensity;
    
    [Space]
    [Header("Moon")] 
    [Space]
    
    public Light moon;
    
    [Space]
    
    public Gradient moonColor;
    
    [Space]
    
    public AnimationCurve moonIntensity;

    [Space]
    [Header("Light Curve")] 
    [Space]

    public AnimationCurve lightningIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    public static TimeSystem instance;


    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        timeRate = 1.0f / fulldayLength;
        
        time = startTime;

    }

    private void Update()
    {
        time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
        {
            time = 0.0f;
        }

        sun.transform.eulerAngles = (time ) * noon * 4.0f;
        moon.transform.eulerAngles = (time -0.5f) * noon * 4.0f;
        
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);
        
        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);
        
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }
 
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(false);
        }
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
        }
        
        RenderSettings.ambientIntensity = lightningIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }
    
}


}
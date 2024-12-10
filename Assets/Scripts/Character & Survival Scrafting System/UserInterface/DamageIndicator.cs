using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LPSurvivalEngine
{
    public class DamageIndicator : MonoBehaviour
    {
    [Space]
    [Header("Damage Indicator")]
    [Space]

    public Image Blood;
    public float fadeTime;

    private Coroutine fadeAway;


    public void Flash()
    {
        if (fadeAway != null)
        {
            StopCoroutine(fadeAway);
        }

        Blood.enabled = true;
        fadeAway = StartCoroutine(FadeAway());

    }

    IEnumerator FadeAway()
    {
        float alphaImage = 1.0f;

        while (alphaImage > 0)
        {
            alphaImage -= (1.0f / fadeTime) * Time.deltaTime;
            yield return null;
        }
        Blood.enabled = false;
        
    }
}


}
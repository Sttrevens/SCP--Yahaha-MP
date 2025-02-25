using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using LPSurvivalEngine;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CustomRendererFeature customRendererFeature;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            customRendererFeature.enableLineStyle = !customRendererFeature.enableLineStyle;
        }
    
        if (Input.GetKeyDown(KeyCode.K))
        {
            customRendererFeature.enableMoHuPostProcessing = !customRendererFeature.enableMoHuPostProcessing;
        }
        
        /*if (GameObject.Find("CurrentPlayer") == null) return;
        
        HealthSystem healthSystem = GameObject.Find("CurrentPlayer").GetComponent<HealthSystem>();
        
        customRendererFeature.enableMoHuPostProcessing = healthSystem.isTired;
        customRendererFeature.enableInvertColor = healthSystem.isScared;;*/
    }

    public void Mohu()
    {
        customRendererFeature.enableMoHuPostProcessing = true;
    }
    
    public void NoMohu()
    {
        customRendererFeature.enableMoHuPostProcessing = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using LPSurvivalEngine;
using UnityEngine;

public class GlobalPostProcessing : MonoBehaviour
{
    public CustomRendererFeature customRendererFeature;
    public static GlobalPostProcessing instance;

    void Awake()
    {
        instance = this;
        ChangeMohuState(false);
        ChangeStateInvert(false);
        ChangeStateLineStyle(false);
    }

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

    public void ChangeMohuState(bool state)
    {
        // customRendererFeature.enableMoHuPostProcessing = state;
    }
    
    public void ChangeStateLineStyle(bool state)
    {
        // customRendererFeature.enableLineStyle = state;
    }

    public void ChangeStateInvert(bool state)
    {
        // customRendererFeature.enableInvertColor = state;
    }
}

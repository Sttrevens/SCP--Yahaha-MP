using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CustomRendererFeature customRendererFeature;

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     Debug.Log("testsssssssssssssssssssssssssssssssssssss");
        //     customRendererFeature.enableLineStyle = !customRendererFeature.enableLineStyle;
        // }

        if (Input.GetKeyDown(KeyCode.K))
        {
            customRendererFeature.enableMoHuPostProcessing = !customRendererFeature.enableMoHuPostProcessing;
        }
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

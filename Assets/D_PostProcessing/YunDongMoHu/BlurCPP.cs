using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CPP.EFFECT
{
    [VolumeComponentMenu(("Custom Post Processings/BlurCpp"))]
    public class BlurCPP : CustomPostProcessing
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter quantizationLevel = new IntParameter(12);
        public override bool IsActive() => m_Material != null && intensity.value != 0;
        private const string mShaderName = "Custom/FatigueVerigo";
        public override CustomPostProcessInjectionPoint InjectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;
    
        public override int OrderInInjectionPoint => 1;
        // 定义Shader中的 PropertyToID
    
        // private const string mTempRTName = "_TempRenderTexture";
        // private RTHandle m_TempRT;
    
        public override void Setup()
        {
            if (m_Material == null)
            {
                m_Material = CoreUtils.CreateEngineMaterial(mShaderName);
            }
        }
    
    
        // public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        // {
        //     //Blur效果应该不需要重新分配RTHandle
        //     var descriptor = getCameraRenderTextureDescriptor(renderingData);
        // }
        public override void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source, in RTHandle destination)
        {
            Debug.Log("[SRP] 覆写Render成功");
            if(m_Material == null) return;  
            Debug.Log("[SRP] setFloat");
            m_Material.SetFloat("_Intensity", intensity.value);
            m_Material.SetInt("_QuantizationNum", quantizationLevel.value);
            Debug.Log("[SRP] 要Draw了");
            Draw(cmd,source,destination,0);
            Debug.Log("[SRP] Draw成功");
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CoreUtils.Destroy(m_Material);
        }
    }

}

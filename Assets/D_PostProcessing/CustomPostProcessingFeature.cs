using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class CustomPostProcessingFeature : ScriptableRendererFeature
{
    private CustomPostProcessingPass mAfterOpaqueAndSkyPass;
    private CustomPostProcessingPass mBeforePostProcessPass;
    private CustomPostProcessingPass mAfterPostProcessPass;

    private List<CustomPostProcessing> mCustomPostProcessings;
    
    public override void Create()
    {
        var stack = VolumeManager.instance.stack;
        mCustomPostProcessings = VolumeManager.instance.baseComponentTypeArray
            .Where(t => t.IsSubclassOf(typeof(CustomPostProcessing)))
            .Select(t => stack.GetComponent(t) as CustomPostProcessing)
            .ToList();
        var afterOpaqueAndSkyCpps = mCustomPostProcessings
            .Where(c =>c.InjectionPoint == CustomPostProcessInjectionPoint.AfterOpaqueAndSkybox)
            .OrderBy(c => c.OrderInInjectionPoint)
            .ToList();
        mAfterOpaqueAndSkyPass = new CustomPostProcessingPass("CPPAfterSkybox", afterOpaqueAndSkyCpps);
        mAfterOpaqueAndSkyPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        
        var beforePostProcessingsCPPs = mCustomPostProcessings
            .Where(c =>c.InjectionPoint == CustomPostProcessInjectionPoint.BeforePostProcess)
            .OrderBy(c => c.OrderInInjectionPoint)
            .ToList();
        mBeforePostProcessPass = new CustomPostProcessingPass("CPPBeforePostProcess", beforePostProcessingsCPPs);
        mAfterOpaqueAndSkyPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
        var afterPostProcessingsCPPs = mCustomPostProcessings
            .Where(c =>c.InjectionPoint == CustomPostProcessInjectionPoint.AfterPostProcess)
            .OrderBy(c => c.OrderInInjectionPoint)
            .ToList();
        mAfterPostProcessPass = new CustomPostProcessingPass("CPPAfterPostProcess", afterPostProcessingsCPPs);
        mAfterOpaqueAndSkyPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled)
        {
            if (mAfterOpaqueAndSkyPass.SetupCustomPostProcessing())
            {
                mAfterOpaqueAndSkyPass.ConfigureInput(ScriptableRenderPassInput.Color);
                renderer.EnqueuePass(mAfterOpaqueAndSkyPass);
            }
            if (mBeforePostProcessPass.SetupCustomPostProcessing())
            {
                mBeforePostProcessPass.ConfigureInput(ScriptableRenderPassInput.Color);
                renderer.EnqueuePass(mBeforePostProcessPass);
            }
            if (mAfterPostProcessPass.SetupCustomPostProcessing())
            {
                mAfterPostProcessPass.ConfigureInput(ScriptableRenderPassInput.Color);
                renderer.EnqueuePass(mAfterPostProcessPass);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        mAfterOpaqueAndSkyPass.Dispose();
        mBeforePostProcessPass.Dispose();
        mAfterPostProcessPass.Dispose();

        if (mCustomPostProcessings != null)
        {
            foreach (var item in mCustomPostProcessings)
            {
                item.Dispose();
            }
        }
    }
}

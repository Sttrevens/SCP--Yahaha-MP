using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();
    // LineStylePass _lineStylePass;
    MoHuPostProcessingPass _mohuPostProcessingPass;
    // public bool enableLineStyle = false;
    public bool enableMoHuPostProcessing = false;
    public override void Create()
    {
        // _lineStylePass = new LineStylePass(RenderPassEvent.BeforeRenderingPostProcessing, settings.shader);
        _mohuPostProcessingPass = new MoHuPostProcessingPass(RenderPassEvent.BeforeRenderingPostProcessing, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // // 根据条件添加不同的 Pass
        // if (enableLineStyle)
        // {
        //     renderer.EnqueuePass(_lineStylePass); // 将 pass1 添加到渲染队列中
        // }

        if (enableMoHuPostProcessing)
        {
            renderer.EnqueuePass(_mohuPostProcessingPass); // 将 pass2 添加到渲染队列中
        }
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // _lineStylePass.Setup(renderer);
        _mohuPostProcessingPass.Setup(renderer);
    }
}

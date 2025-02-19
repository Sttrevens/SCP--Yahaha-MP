using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoHuPostProcessingFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();
    MoHuPostProcessingPass pass;
    public override void Create()
    {
        this.name = "MoHuPostProcessingPass";
        pass = new MoHuPostProcessingPass(RenderPassEvent.BeforeRenderingPostProcessing, settings.shader);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        pass.Setup(renderer);
    }
}

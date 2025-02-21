using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InvertColorPass : ScriptableRenderPass
{
    static readonly string renderTag = "Invert Color Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetColorTint");
    
    private InvertColor invertColorVolume;
    private Material mat;
    RenderTargetIdentifier currentTarget;
    
    public InvertColorPass(RenderPassEvent passEvent, Shader invertColorShader = null)
    {
        renderPassEvent = passEvent;
        invertColorShader = Shader.Find("Post Process/Invert Color");
        if (invertColorShader == null)
        {
            Debug.Log("没找到反转色彩的shader");
            return;
        }
        mat = CoreUtils.CreateEngineMaterial(invertColorShader);
    }
    public void Setup(ScriptableRenderer renderer){
        currentTarget = renderer.cameraColorTargetHandle;
    }
    private void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref CameraData cameraData = ref renderingData.cameraData;
        Camera camera = cameraData.camera;
        RenderTargetIdentifier source = currentTarget;
        int destination = TempTargetId;
        
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight, 0, FilterMode.Trilinear, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, mat, 0);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(mat == null) return;
        if(!renderingData.cameraData.postProcessEnabled) return;
        VolumeStack stack = VolumeManager.instance.stack;
        invertColorVolume = stack.GetComponent<InvertColor>();
        if (invertColorVolume == null)
        {
            Debug.Log("反转颜色后处理没有挂在Volume上");
            return;
        }
        CommandBuffer cmd = CommandBufferPool.Get(renderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
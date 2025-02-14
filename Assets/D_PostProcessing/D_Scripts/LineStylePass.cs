using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LineStylePass : ScriptableRenderPass
{
    static readonly string renderTag = "LineStyle Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetColorTint");

    private LineStyle lineStyleVolume;
    private Material mat;
    RenderTargetIdentifier currentTarget;

    public LineStylePass(RenderPassEvent passEvent,Shader lineStyleShader)
    {
        renderPassEvent = passEvent;
        if(lineStyleShader == null)
        {
            Debug.LogError("Shader不存在");
            return;
        }
        mat = CoreUtils.CreateEngineMaterial(lineStyleShader);
    }

    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(mat == null)
        {
            return;
        }
        if(!renderingData.cameraData.postProcessEnabled)
        {
            return;
        }
        VolumeStack stack = VolumeManager.instance.stack;
        lineStyleVolume = stack.GetComponent<LineStyle>();
        if(lineStyleVolume == null)
        {
            return;
        }
        if (lineStyleVolume.isShow.value == false)
        {
            return;
        }
        CommandBuffer cmd = CommandBufferPool.Get(renderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd,ref RenderingData renderingData)
    {

        ref CameraData cameraData = ref renderingData.cameraData;
        Camera camera = cameraData.camera;
        RenderTargetIdentifier source = currentTarget;
        int destination = TempTargetId;

        mat.SetFloat("_lineStrength", lineStyleVolume.lineStrength.value);
        mat.SetColor("_lineColor", lineStyleVolume.lineColor.value);
        mat.SetColor("_baseColor", lineStyleVolume.baseColor.value);

        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight, 0, FilterMode.Trilinear, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, mat, 0);
    }
}

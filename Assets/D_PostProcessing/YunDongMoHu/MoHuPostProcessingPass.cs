using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MoHuPostProcessingPass : ScriptableRenderPass
{
    static readonly string RenderTag = "MoHuPostProcessing";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetColorTint");
    private MoHuPostProcessing MoHuPostProcessingVolume;
    private Material mat;
    RenderTargetIdentifier currentTarget;

    public MoHuPostProcessingPass(RenderPassEvent passEvent, Shader moHuPostProcessingShader = null)
    {
        Debug.Log("MoHuPostProcessingPass进入喵喵喵喵喵喵");
        renderPassEvent = passEvent;
        moHuPostProcessingShader = Shader.Find("Post Process/Invert Color");
        if (moHuPostProcessingShader == null)
        {
            Debug.Log("没找到shader");
            return;
        }
        mat = CoreUtils.CreateEngineMaterial(moHuPostProcessingShader);
    }
    public void Setup(ScriptableRenderer renderer)
    {
        currentTarget = renderer.cameraColorTargetHandle;
    }

    private void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref CameraData cameraData = ref renderingData.cameraData;
        Camera camera = cameraData.camera;
        RenderTargetIdentifier source = currentTarget;
        int destination = TempTargetId;
        
        // mat.SetFloat("_DistortionStrength",MoHuPostProcessingVolume.distortionStrength.value);
        // mat.SetFloat("_DistortionSpeed", MoHuPostProcessingVolume.distortionSpeed.value);
        // 把现在摄像机渲染的纹理（当前帧的画面）绑定到一个全局的shader属性变量
        cmd.SetGlobalTexture(MainTexId, source);
        // 创建一个临时的Render Texture，用作中间渲染目标 这个临时生成的纹理就是这个destination，只是一个数字ID
        cmd.GetTemporaryRT(destination, cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight, 0, FilterMode.Trilinear, RenderTextureFormat.Default);
        // 从当前渲染帧的画面（`source`）渲染到一个临时渲染目标（`destination`），为后续处理做准备。
        cmd.Blit(source, destination);
        // 再次使用 `Blit`，将 `destination` 纹理通过一个自定义材质 `mat` 渲染回去到 `source`。
        cmd.Blit(destination, source, mat, 0);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (mat == null)
        {
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            return;
        }
        VolumeStack stack = VolumeManager.instance.stack;
        MoHuPostProcessingVolume = stack.GetComponent<MoHuPostProcessing>();
        if(MoHuPostProcessingVolume == null)
        {
            Debug.Log("没找着模糊后处理的喵喵喵喵喵喵喵喵");
            return;
        }
        CommandBuffer cmd = CommandBufferPool.Get(RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}



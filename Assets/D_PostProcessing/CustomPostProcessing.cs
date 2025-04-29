using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public enum CustomPostProcessInjectionPoint
{
    AfterOpaqueAndSkybox,
    BeforePostProcess,
    AfterPostProcess
}
public abstract class CustomPostProcessing : VolumeComponent , IPostProcessComponent,IDisposable
{
    // 首先定义两个材质
    protected Material m_Material = null;
    private Material mCopyMaterial = null;
     // 这个Shader什么含义还没弄懂 （这个Shader是搬运工）
    private const string mCopyShaderName = "Hidden/PostProcess/PostProcessCopy";
    // 注入点
    public virtual CustomPostProcessInjectionPoint InjectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;
    // 在注入点中的顺序
    public virtual int OrderInInjectionPoint => 0;
    // 后处理enable的处理逻辑
    protected override void OnEnable()
    {
        base.OnEnable();
        if (mCopyMaterial == null)
        {
            mCopyMaterial = CoreUtils.CreateEngineMaterial(mCopyShaderName);
        }
    }

    #region Setup
    //处理初始化的逻辑
    public abstract bool IsActive();

    public abstract void Setup();

    public virtual void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        
    }

    #endregion

    #region Render
    // 渲染有关
    public abstract void Render(CommandBuffer cmd, ref RenderingData renderingData, in RTHandle source,
        in RTHandle destination);
    
    private int mSourceTextureId = Shader.PropertyToID("_SourceTexture");

    public virtual void Draw(CommandBuffer cmd, in RTHandle source, in RTHandle destination, int pass = -1)
    {
        // 设置一个全局的纹理变量
        cmd.SetGlobalTexture(mSourceTextureId, source);
        // 
        CoreUtils.SetRenderTarget(cmd, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        if (pass == -1 || m_Material == null)
        {
            cmd.DrawProcedural(Matrix4x4.identity, mCopyMaterial, 0, MeshTopology.Triangles, 3);
        }
        else
        {
            cmd.DrawProcedural(Matrix4x4.identity, m_Material, pass, MeshTopology.Triangles, 3);
        }
    }

    protected RenderTextureDescriptor getCameraRenderTextureDescriptor(RenderingData renderingData)
    {
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        descriptor.msaaSamples = 1;
        descriptor.useMipMap = false;
        return descriptor;
    }

    protected void SetKeyword(string keyword, bool enabled = true)
    {
        if (enabled)
        {
            m_Material.EnableKeyword(keyword);
        }
        else
        {
            m_Material.DisableKeyword(keyword);
        }
    }

    #endregion
    public virtual bool IsTileCompatible() => false;
    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        System.GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        
    }
    #endregion
}

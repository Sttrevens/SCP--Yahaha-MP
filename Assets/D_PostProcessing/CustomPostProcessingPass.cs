using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class CustomPostProcessingPass : ScriptableRenderPass
{
    private List<CustomPostProcessing> mCustomPostProcessings;
    private List<int> mActiveCustomPostProcessingIndex;
    
    private string mProfilerTag;
    private List<ProfilingSampler> mProfilingSamplers;

    private RTHandle mSourceRT;
    private RTHandle mDesRT;
    private RTHandle mTempRT0;
    private RTHandle mTempRT1;
    
    private string mTempRT0Name => "_TemporaryRT0";
    private string mTempRT1Name => "_TemporaryRT1";
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        descriptor.msaaSamples = 1;
        
        RenderingUtils.ReAllocateIfNeeded(ref mTempRT0, descriptor, name:mTempRT0Name);
        RenderingUtils.ReAllocateIfNeeded(ref mTempRT1, descriptor, name:mTempRT1Name);
        foreach (var i in mActiveCustomPostProcessingIndex)
        {
            mCustomPostProcessings[i].OnCameraSetup(cmd, ref renderingData);
        }
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        mDesRT = null;
        mSourceRT = null;
    }

    

    public CustomPostProcessingPass(string profilerTag, List<CustomPostProcessing> customPostProcessings)
    {
        mProfilerTag = profilerTag;
        mCustomPostProcessings = customPostProcessings;
        mActiveCustomPostProcessingIndex = new List<int>(customPostProcessings.Count);
        mProfilingSamplers = customPostProcessings.Select(c => new ProfilingSampler(c.ToString())).ToList();
        
        mTempRT0 = RTHandles.Alloc(mTempRT0Name,name: mTempRT0Name);
        mTempRT1 = RTHandles.Alloc(mTempRT1Name,name: mTempRT1Name);
    }

    public bool SetupCustomPostProcessing()
    {
        mActiveCustomPostProcessingIndex.Clear();
        for (int i = 0; i < mCustomPostProcessings.Count; i++)
        {
            mCustomPostProcessings[i].Setup();
            if (mCustomPostProcessings[i].IsActive())
            {
                mActiveCustomPostProcessingIndex.Add(i);
            }
        }
        return mActiveCustomPostProcessingIndex.Count != 0;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get(mProfilerTag);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        mDesRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
        mSourceRT = renderingData.cameraData.renderer.cameraColorTargetHandle;

        if (mActiveCustomPostProcessingIndex.Count == 1)
        {
            int index = mActiveCustomPostProcessingIndex[0];
            using (new ProfilingScope(cmd , mProfilingSamplers[index]))
            {
                mCustomPostProcessings[index].Render(cmd, ref renderingData,mSourceRT,mTempRT0);
            }
        }
        else
        {
            Blitter.BlitCameraTexture(cmd, mSourceRT, mTempRT0);
            for (int i = 0; i < mActiveCustomPostProcessingIndex.Count; i++)
            {
                int index = mActiveCustomPostProcessingIndex[i];
                var customPostProcessing = mCustomPostProcessings[index];
                using (new ProfilingScope(cmd , mProfilingSamplers[index]))
                {
                    customPostProcessing.Render(cmd, ref renderingData,mSourceRT,mTempRT1);
                }
                CoreUtils.Swap(ref mTempRT0 , ref mTempRT1);
            }
        }
        Blitter.BlitCameraTexture(cmd, mTempRT0, mDesRT);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        mTempRT0?.Release();
        mTempRT1?.Release();
    }
}

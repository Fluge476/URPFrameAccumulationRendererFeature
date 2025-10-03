using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class FrameAccumulationRendererFeature : ScriptableRendererFeature
{


    public RenderPassEvent injectionPoint;
    
    class AccumulationPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private RTHandle accumulationBuffer;
        
        public AccumulationPass(Material blitMaterial)
        {
            this.blitMaterial = blitMaterial;
        }
        
        void ReAllocate(RenderTextureDescriptor desc)
        {
            desc.msaaSamples = 1;
            desc.depthStencilFormat = GraphicsFormat.None;
            RenderingUtils.ReAllocateHandleIfNeeded(ref accumulationBuffer, desc, name: "_AccumulationBuffer");
        }
        
        // This class stores the data needed by the pass, passed as parameter to the delegate function that executes the pass
        private class PassData
        {
            internal TextureHandle src;
            internal TextureHandle dst;
            internal Material blitMaterial;
        }
        
        public class FrameBufferData : ContextItem 
        {
            public TextureHandle accumulationTextureHandle;
            public override void Reset()
            {
                accumulationTextureHandle = TextureHandle.nullHandle;
            }
        }

        // This static method is used to execute the pass and passed as the RenderFunc delegate to the RenderGraph render pass
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
        }

        public void Dispose()
        {
            accumulationBuffer?.Release();
        }
        
        private void InitPassData(RenderGraph renderGraph, ContextContainer frameData, ref PassData passData)
        {

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            
            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 0;
            desc.depthBufferBits = 0;
                
            ReAllocate(desc);
            TextureHandle destination = renderGraph.ImportTexture(accumulationBuffer);
            FrameBufferData customData = frameData.Create<FrameBufferData>();
            customData.accumulationTextureHandle = destination;
            
            passData.src = resourceData.activeColorTexture;
            passData.dst = destination;
            passData.blitMaterial = blitMaterial;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            string passName = "First Blur Pass (with material i mean tbh lol hehehehehe)";
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                InitPassData(renderGraph, frameData, ref passData);
                builder.UseTexture(passData.src);
                builder.SetRenderAttachment(passData.dst, 0);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }
    }
    class BlitForwardPass : ScriptableRenderPass
    {
        private class PassData
        {
            internal TextureHandle src;
            internal TextureHandle dst;
        }
        
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
        }

        private void InitPassData(RenderGraph renderGraph, ContextContainer frameData, ref PassData passData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            var frameBufferData = frameData.Get<AccumulationPass.FrameBufferData>();
            var blurredBuffer = frameBufferData.accumulationTextureHandle;
            passData.src = blurredBuffer;
            passData.dst = resourceData.activeColorTexture;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            string passName = "Second Blur Pass";
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                InitPassData(renderGraph, frameData, ref passData);
                builder.UseTexture(passData.src);
                builder.SetRenderAttachment(passData.dst, 0);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }
    }

    AccumulationPass blurBackBufferPass;
    BlitForwardPass blitForwardPass;
    
    public Material blitColorMaterial;
    
    public override void Create()
    {
        blurBackBufferPass = new AccumulationPass(blitColorMaterial);
        blitForwardPass = new BlitForwardPass();
        blurBackBufferPass.renderPassEvent = injectionPoint;
        blitForwardPass.renderPassEvent = injectionPoint;
    }
    
    protected override void Dispose(bool disposing)
    {
        blurBackBufferPass.Dispose();
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(blurBackBufferPass);
        renderer.EnqueuePass(blitForwardPass);
    }
}
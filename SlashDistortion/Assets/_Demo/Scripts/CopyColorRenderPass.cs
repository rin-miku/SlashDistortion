using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CopyColorRenderPass : ScriptableRenderPass
{
    private Material blitMaterial;

    private class PassData
    {
        public TextureHandle source;
    }

    public CopyColorRenderPass()
    {
        if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out var settings))
        {
            blitMaterial = CoreUtils.CreateEngineMaterial(settings.coreBlitPS);
        }
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
        desc.depthStencilFormat = GraphicsFormat.None;
        desc.msaaSamples = 1;
        desc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
        TextureHandle tempColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_CameraColorTexture", true);

        using (var builder = renderGraph.AddRasterRenderPass("CopyColor", out PassData passData))
        {
            passData.source = resourceData.activeColorTexture;

            builder.AllowPassCulling(false);
            builder.SetRenderAttachment(tempColor, 0);
            builder.UseTexture(passData.source);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), blitMaterial, 0);
            });

            builder.SetGlobalTextureAfterPass(tempColor, Shader.PropertyToID("_CameraColorTexture"));
        }
    }
}

using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class SlashDistortionRenderPass : ScriptableRenderPass
{
    private class PassData
    {
        public RendererListHandle rendererListHandle;
    }

    public SlashDistortionRenderPass()
    {

    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Slash Distortion", out var passData))
        {
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            SortingCriteria sortFlags = cameraData.defaultOpaqueSortFlags;
            RenderQueueRange renderQueueRange = RenderQueueRange.transparent;
            FilteringSettings filterSettings = new FilteringSettings(renderQueueRange, ~0);

            ShaderTagId shadersToOverride = new ShaderTagId("SlashDistortion");

            DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shadersToOverride, renderingData, cameraData, lightData, sortFlags);

            var rendererListParameters = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);

            passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParameters);

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            builder.UseRendererList(passData.rendererListHandle);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.rendererListHandle);
            });
        }
    }
}

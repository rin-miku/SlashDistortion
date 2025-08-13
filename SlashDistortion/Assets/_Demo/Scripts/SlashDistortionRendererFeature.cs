using UnityEngine.Rendering.Universal;

public class SlashDistortionRendererFeature : ScriptableRendererFeature
{
    private CopyColorRenderPass copyColorRenderPass;
    private SlashDistortionRenderPass slashDistortionRenderPass;

    public override void Create()
    {
        copyColorRenderPass = new CopyColorRenderPass();
        copyColorRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        slashDistortionRenderPass = new SlashDistortionRenderPass();
        slashDistortionRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(copyColorRenderPass);
        renderer.EnqueuePass(slashDistortionRenderPass);
    }
}

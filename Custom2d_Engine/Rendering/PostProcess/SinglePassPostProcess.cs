using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Rendering.PostProcess;

public class SinglePassPostProcess : IPostProcess {

    public int Pass { get; set; }

    private readonly Effect _effect;
    
    public SinglePassPostProcess(Effect effect) {
        this._effect = effect;
    }

    public Texture2D Render(RenderPipeline pipeline, Texture2D scene) {
        using var _ = new RenderPipeline.EffectScope(pipeline, _effect);
        pipeline.Rendering.DrawFullTex(scene, Pass);
        return pipeline.RenderTarget.FinishPass();
    }
    
}
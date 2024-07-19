using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Rendering.PostProcess;

public interface IPostProcess {

    public Texture2D Render(RenderPipeline pipeline, Texture2D scene);
    
}
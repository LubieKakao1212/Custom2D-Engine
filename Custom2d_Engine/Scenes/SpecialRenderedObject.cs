using Microsoft.Xna.Framework;
using Custom2d_Engine.Rendering;

namespace Custom2d_Engine.Scenes {
    public abstract class SpecialRenderedObject : DrawableObject, ISpecialRenderer {
        protected RenderPipeline Pipeline { get; }

        public SpecialRenderedObject(RenderPipeline pipeline, Color color, float drawOrder) : base(color, drawOrder) {
            this.Pipeline = pipeline;
            this.SetInterruptQueue(true);
        }

        public abstract void Render(Camera camera);
    }
}
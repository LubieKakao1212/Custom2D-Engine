using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes
{
    public abstract class SpecialRenderedObject : DrawableObject, ISpecialRenderer
    {
        protected RenderPipeline Pipeline { get; }

        public bool AllowFrameAccess { get; protected set; }

        public SpecialRenderedObject(RenderPipeline pipeline, Color color, float drawOrder) : base(color, drawOrder) 
        { 
            this.Pipeline = pipeline;
            SetQueueBehaviour(RenderPasses.Normals, QueueBehaviour.Skip);
            SetQueueBehaviour(RenderPasses.Lights, QueueBehaviour.Skip);
            SetQueueBehaviour(RenderPasses.Final, QueueBehaviour.Skip);
        }

        protected virtual void RenderFinal(Texture2D sceneLights) { }
        protected virtual void RenderNormals() { }
        protected virtual void RenderLights(Texture2D sceneNormals) { }

        public override DrawableObject SetQueueBehaviour(RenderPasses pass, QueueBehaviour behaviour)
        {
            this.PassQueueBehaviours[(byte)pass] = behaviour;
            return this;
        }

        public void Render(RenderPasses pass, Texture2D previousPassResultTexture)
        {
            switch (pass)
            {
                case RenderPasses.Normals:
                    RenderNormals();
                    break;
                case RenderPasses.Lights:
                    RenderLights(previousPassResultTexture);
                    break;
                case RenderPasses.Final:
                    RenderFinal(previousPassResultTexture);
                    break;
            }
        }
    }
}

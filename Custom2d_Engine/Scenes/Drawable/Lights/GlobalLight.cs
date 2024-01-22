using Custom2d_Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes.Drawable.Lights
{
    public class GlobalLight : BaseLight
    {
        public GlobalLight(RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {

        }

        protected override Effect InitEffect()
        {
            return Effects.Lights.GlobalLight.Clone();
        }

        //TODO Transform Objects normal Map
        protected override void DoLight(Effect effect)
        {
            var dir = Transform.Up;
            dir = Pipeline.CurrentState.CurrentProjection.TransformDirection(dir);
            effect.Parameters[Effects.Lights.Direction]?.SetValue(dir);
            Pipeline.Rendering.DrawFull(effect);
        }
    }
}

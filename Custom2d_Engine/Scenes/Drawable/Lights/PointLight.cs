using Custom2d_Engine.Math;
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
    public class PointLight : BaseLight
    {
        public float InnerRadius
        {
            get => innerRadius;
            set
            {
                innerRadius = value;
                SetDirty();
            }
        }
        public float OuterRadius
        {
            get => outerRadius;
            set
            {
                outerRadius = value;
                SetDirty();
            }
        }

        public float InnerAngle
        {
            get => innerAngle;
            set
            {
                innerAngle = value;
                SetDirty();
            }
        }
        public float OuterAngle
        {
            get => outerAngle;
            set
            {
                outerAngle = value;
                SetDirty();
            }
        }

        private float innerRadius = 2f;
        private float outerRadius = 2f;
        private float innerAngle = MathHelper.PiOver4;
        private float outerAngle = MathHelper.PiOver2;

        public PointLight(RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {
        }

        protected override bool Cull()
        {
            /*var proj = Pipeline.CurrentState.CurrentProjection;
            var LtW = Transform.LocalToWorld;
            var rect = BoundingRect.Normal;
            rect.Transform(LtW);*/
            return true;
        }

        protected override void UpdateParams(EffectParameterCollection parameters)
        {
            base.UpdateParams(parameters);
            parameters[Effects.Lights.InnerRadiusRatio].SetValue(innerRadius / outerRadius);
            parameters[Effects.Lights.OutInAngleRatio].SetValue(new Vector2(outerAngle / MathHelper.Tau, innerAngle / outerAngle));
        }

        protected override void DoLight(Effect effect)
        {
            var dir = Transform.Up;
            //dir = Pipeline.CurrentState.CurrentProjection.TransformDirection(dir);
            effect.Parameters[Effects.Lights.Direction]?.SetValue(dir);
            effect.Parameters[Effects.Lights.ObjWorldPos].SetValue(Transform.GlobalPosition);
            
            Pipeline.Rendering.DrawQuad(Transform.LocalToWorld * new Matrix2x2(outerRadius));
        }

        protected override Effect InitEffect()
        {
            return Effects.Lights.PointLight.Clone();
        }
    }
}

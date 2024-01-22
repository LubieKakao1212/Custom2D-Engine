using Custom2d_Engine.Rendering;
using Custom2d_Engine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Scenes.Drawable.Lights
{
    public abstract class BaseLight : SpecialRenderedObject
    {
        public float Intensity 
        { 
            get => intensity; 
            set
            {
                intensity = value;
                SetDirty();
            }
        }

        public float LightHeight
        {
            get => lightHeight;
            set 
            {
                lightHeight = value;
                SetDirty();
            }
        }

        private float intensity;
        private float lightHeight;

        private Effect LightEffect
        {
            get
            {
                return lightEffect ??= InitEffect();
            }
        }

        private Effect lightEffect;

        private bool dirty = true;
        private UpdateListener<Color> colorListener;

        public BaseLight(RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {
            SetQueueBehaviour(RenderPasses.Lights, QueueBehaviour.CustomDraw);
        }

        protected abstract Effect InitEffect();

        protected void SetDirty()
        {
            dirty = true;
        }

        protected virtual void UpdateParams(EffectParameterCollection parameters)
        {
            parameters[Effects.Lights.Intensity]?.SetValue(Intensity);
            parameters[Effects.Lights.Tint]?.SetValue(Color.ToVector4());
            parameters[Effects.Lights.Height]?.SetValue(lightHeight);
        }

        protected override void RenderLights(Texture2D sceneNormals)
        {
            var effect = LightEffect;
            var parameters = effect.Parameters;
            if (dirty || colorListener.WasUpdated(Color))
            {
                UpdateParams(effect.Parameters);
            }
            parameters[Effects.SceneNormals]?.SetValue(sceneNormals);
            DoLight(effect);
        }

        protected abstract void DoLight(Effect effect);

        protected override void RemovedFromScene()
        {
            base.RemovedFromScene();
            lightEffect?.Dispose();
            lightEffect = null;
        }
    }
}

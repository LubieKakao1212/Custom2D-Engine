
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Rendering.PostProcess;

public class BloomPostProcess : IPostProcess {

    public float Sigma {
        get => blur.Sigma;
        set => blur.Sigma = value;
    }
    
    public int KernelRadius {
        get => blur.KernelRadius;
        set => blur.KernelRadius = value;
    }
    
    public float Threshold { get; set; } = 1f;
    private readonly Effect Extract = Effects.Extract.Clone();
    private readonly BlurPostProcess blur = new BlurPostProcess();

    private bool dirty;
    
    public Texture2D Render(RenderPipeline pipeline, Texture2D scene) {
        if (dirty) {
            Extract.Parameters[Effects.ThresholdParameter].SetValue(Threshold);
            //Blur.Parameters[Effects.KernelParameter].SetValue(MakeKernel(KernelRadius, Sigma));
        }

        return null;
    }

    public void SetDirty() {
        dirty = true;
        blur.SetDirty();
    }
    
    public static class Effects {
        private const string ExtractEffectPath = "BloomExtract";
        private const string CombineEffectPath = "BloomCombine";

        public const string ThresholdParameter = "threshold";

        public static Effect Extract { get; private set; }
        public static Effect Combine { get; private set; }
        
        public static void Init(ContentManager content, string extract = null, string combine = null) {
            Extract = content.Load<Effect>(extract ?? ExtractEffectPath);
            Combine = content.Load<Effect>(combine ?? CombineEffectPath);
        }
    }
}
using System;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Rendering.PostProcess;

public class BlurPostProcess : IPostProcess {
    
    public int KernelRadius { get; set; } = 3;
    public float Sigma { get; set; } = 2f;
    private readonly Effect Blur = Effects.Blur.Clone();
    
    private bool dirty = true;
    
    public Texture2D Render(RenderPipeline pipeline, Texture2D scene) {
        if (dirty) {
            Blur.Parameters[Effects.KernelParameter]?.SetValue(MakeKernel(KernelRadius, Sigma));
            dirty = false;
        }
        
        Blur.Parameters[Effects.ResolutionInverse]?.SetValue(1f / scene.Width);
        
        using var _ = new RenderPipeline.EffectScope(pipeline, Blur);
        pipeline.Rendering.DrawFullTex(scene, 0);
        scene = pipeline.RenderTarget.FinishPass();
        
        Blur.Parameters[Effects.ResolutionInverse]?.SetValue(1f / scene.Height);
        pipeline.Rendering.DrawFullTex(scene, 1);
        return pipeline.RenderTarget.FinishPass();
    }

    public void SetDirty() {
        dirty = true;
    }
    
    public static float[] MakeKernel(int size, float sigma) {
        var arr = Enumerable.Range(0, size).Select(v => MathF.Exp(-(v * v) / (sigma * sigma))).ToArray();
        //*2 => two sides
        //-1 => dont count the center twice
        var sum = arr.Sum() * 2f - 1f;
        
        return arr.Select(v => v / sum).ToArray();
    }
    
    public static class Effects {
        private const string BlurEffectPath = "GaussianBlur";
        
        public const string KernelParameter = "kernel";
        public const string ResolutionInverse = "resInv";
        public const string PixelDelta = "delta";
        
        public static Effect Blur { get; private set; }
        
        public static void Init(ContentManager content, string blur = null) {
            Blur = content.Load<Effect>(blur ?? BlurEffectPath);
        }
        
    }
    
}
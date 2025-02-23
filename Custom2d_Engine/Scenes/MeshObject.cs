using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace Custom2d_Engine.Scenes {
    public class MeshObject : SpecialRenderedObject {
        [NotNull] private Effect? Effect { get; set; }
        [NotNull] private DepthStencilState? DepthStencilState { get; set; }
        [NotNull] private IndexBuffer? Ib { get; set; }
        [NotNull] private VertexBuffer? Vb { get; set; }
        private int PCount { get; set; }

        public static MeshObject CreateNew<T>(RenderPipeline pipeline, VertexDeclaration vertexDeclaration,
            T[] verticies, int[] indicies, Color color, float drawOrder, Effect effect,
            DepthStencilState depthStencilState) where T : struct {
            var mo = new MeshObject(pipeline, color, drawOrder) {
                Vb = new VertexBuffer(pipeline.Graphics, vertexDeclaration, verticies.Length, BufferUsage.WriteOnly),
                Ib = new IndexBuffer(pipeline.Graphics, IndexElementSize.ThirtyTwoBits, indicies.Length,
                    BufferUsage.WriteOnly)
            };
            mo.Vb.SetData(verticies);

            mo.Ib.SetData(indicies);

            mo.PCount = indicies.Length / 3;

            mo.Effect = effect;
            mo.DepthStencilState = depthStencilState;

            return mo;
        }

        private MeshObject(RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder) {
        }

        public override void Render(Camera camera) {
            Effect.CurrentTechnique = Effect.Techniques[0];

            //TODO integrate into pipeline
            var proj = camera.ProjectionMatrix;
            Effect.Parameters[Effects.CameraRS].SetValue(proj.RS.Flat);
            Effect.Parameters[Effects.CameraT].SetValue(proj.T);

            var ltw = Transform.LocalToWorld;
            Effect.Parameters[Effects.ObjRSS].SetValue(ltw.RS.Flat);
            Effect.Parameters[Effects.ObjT].SetValue(ltw.T);

            var dss = Pipeline.Graphics.DepthStencilState;

            Pipeline.Graphics.BlendState = BlendState.NonPremultiplied;
            Pipeline.Graphics.DepthStencilState = DepthStencilState;
            Pipeline.Graphics.Indices = Ib;

            Effect.CurrentTechnique.Passes[0].Apply();

            Pipeline.Graphics.SetVertexBuffer(Vb);

            Pipeline.Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PCount);
            Pipeline.Graphics.DepthStencilState = dss;
        }

        public void UpdateMesh<T>(T[] verticies, int[] indicies) where T : struct {
            Vb.SetData(verticies);
            Ib.SetData(indicies);
        }
    }
}
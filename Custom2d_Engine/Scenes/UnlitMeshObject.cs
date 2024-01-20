using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes
{
    public class UnlitMeshObject : SpecialRenderedObject
    {
        private Effect effect;
        private DepthStencilState depthStencilState;
        private IndexBuffer ib;
        private VertexBuffer vb;
        private int pCount;
        
        public static UnlitMeshObject CreateNew<T>(RenderPipeline pipeline, VertexDeclaration vertexDeclaration, T[] verticies, int[] indicies, Color color, float drawOrder, Effect effect, DepthStencilState depthStencilState) where T : struct
        {
            var mo = new UnlitMeshObject(pipeline, color, drawOrder);
            mo.vb = new VertexBuffer(pipeline.Graphics, vertexDeclaration, verticies.Length, BufferUsage.WriteOnly);
            mo.vb.SetData(verticies);
            
            mo.ib = new IndexBuffer(pipeline.Graphics, IndexElementSize.ThirtyTwoBits, indicies.Length, BufferUsage.WriteOnly);
            mo.ib.SetData(indicies);

            mo.pCount = indicies.Length / 3;

            mo.effect = effect;
            mo.depthStencilState = depthStencilState;

            return mo;
        }

        private UnlitMeshObject(RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {
            SetQueueBehaviour(RenderPasses.Final, QueueBehaviour.CustomDraw);
        }
        
        protected override void RenderFinal(Texture2D _)
        {
            effect.CurrentTechnique = effect.Techniques[0];

            //TODO integrate into pipeline
            var proj = Pipeline.CurrentState.CurrentProjection;
            effect.Parameters[Effects.CameraRS].SetValue(proj.RS.Flat);
            effect.Parameters[Effects.CameraT].SetValue(proj.T);

            var ltw = Transform.LocalToWorld;
            effect.Parameters[Effects.ObjRSS].SetValue(ltw.RS.Flat);
            effect.Parameters[Effects.ObjT].SetValue(ltw.T);

            var dss = Pipeline.Graphics.DepthStencilState;
            
            Pipeline.Graphics.BlendState = BlendState.AlphaBlend;
            Pipeline.Graphics.DepthStencilState = depthStencilState;
            Pipeline.Graphics.Indices = ib;

            effect.CurrentTechnique.Passes[0].Apply();

            Pipeline.Graphics.SetVertexBuffer(vb);

            Pipeline.Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, pCount);
            Pipeline.Graphics.DepthStencilState = dss;
        }

        public void UpdateMesh<T>(T[] verticies, int[] indicies) where T : struct
        {
            vb.SetData(verticies);
            ib.SetData(indicies);
        }
    }
}

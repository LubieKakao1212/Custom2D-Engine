using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering;
using System;
using System.Linq;
using Custom2d_Engine.Scenes.Drawable;

using static Custom2d_Engine.Rendering.RenderPipeline;
using System.Collections.Generic;
using Custom2d_Engine.Scenes;

namespace Custom2d_Engine.Tilemap
{
    public class TilemapRenderer : SpecialRenderedObject
    {
        //TODO make this non static
        private static VertexBuffer chunkTransformBuffer;
        private static VertexBufferBinding chunkTransformBufferBinding;

        private Tilemap<InstanceSpriteData> tilemap;
        private Grid grid;

        public Vector2 spacing { get; set; } = Vector2.One;

        public TilemapRenderer(Tilemap<InstanceSpriteData> tilemap, Grid grid, RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {
            this.tilemap = tilemap;
            this.grid = grid;
            if (chunkTransformBuffer == null)
            {
                CreateCTB();
            }

            SetQueueBehaviour(RenderPasses.Normals, QueueBehaviour.CustomDraw);
            SetQueueBehaviour(RenderPasses.Final, QueueBehaviour.CustomDraw);
        }

        private void RenderChunks(RenderPasses pass, Texture2D sceneLights)
        {
            var chunkSize = Chunk<InstanceSpriteData>.chunkSize;
            
            var gridWtL = Matrix2x2.Scale(grid.CellSize * 2f).Inverse() * grid.Transform.WorldToLocal;
            gridWtL = gridWtL.Translate(Vector2.One * (chunkSize / 4f));

            var state = Pipeline.CurrentState;
            var projection = state.CurrentProjection;

            var bounds = Camera.CullReverse(projection, gridWtL);
            
            var effect = Effects.TilemapDefault;

            var parameters = effect.Parameters;
            parameters[Effects.Spacing].SetValue(spacing);
            parameters[Effects.ChunkRS].SetValue(grid.Transform.LocalToWorld.RS.Flat);
            if (sceneLights != null)
            {
                parameters[Effects.SceneLights]?.SetValue(sceneLights);
            }

            using var effectScope = new EffectScope(Pipeline, effect);

            foreach (var chunk in tilemap.GetChunksAt(bounds.ToInt(), false))
            {
                var crd = chunk.RenderData;
                if (crd == null)
                {
                    crd = new ChunkRenderData(chunk, Pipeline);
                    chunk.RenderData = crd;
                }
                //TODO detect changes instead of constantly flushing
                crd.Flush();

                var cPos = chunk.ChunkPos.ToVector2() * spacing * (chunkSize * 2f);
                cPos = grid.Transform.LocalToWorld.TransformPoint(cPos);

                parameters[Effects.ChunkT].SetValue(cPos);

                Pipeline.Rendering.DrawInstancedQuads(chunkSize * chunkSize, pass.GetShaderPasssIdx(), crd.Binding, chunkTransformBufferBinding);
            }
        }

        protected override void RenderFinal(Texture2D sceneLights)
        {
            RenderChunks(RenderPasses.Final, sceneLights);
        }

        protected override void RenderNormals()
        {
            RenderChunks(RenderPasses.Normals, null);
        }

        public void CreateCTB()
        {
            var chunkSize = Chunk<InstanceSpriteData>.chunkSize;
            var tileCount = chunkSize * chunkSize;
            var rotScale = new Matrix2x2(1f).Flat;

            var bufferData = new InstanceTransformData[tileCount];

            var offset = new Vector2(chunkSize / 2f, chunkSize / 2f); 

            for (int y = 0; y < chunkSize; y++)
                for (int x = 0; x < chunkSize; x++)
                {
                    var data = default(InstanceTransformData);
                    data.rotScale = rotScale;
                    data.pos = (new Vector2(x, y) * 2f) - offset;
                    bufferData[y * chunkSize + x] = data;
                }

            chunkTransformBuffer = new VertexBuffer(Pipeline.Graphics, InstanceTransformData.VertexDeclaration, tileCount, BufferUsage.WriteOnly);
            chunkTransformBuffer.SetData(bufferData);

            chunkTransformBufferBinding = new VertexBufferBinding(chunkTransformBuffer, 0, 1);
        }
    }
}

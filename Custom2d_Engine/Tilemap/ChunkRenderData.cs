using Custom2d_Engine.Rendering;
using Custom2d_Engine.Scenes.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Custom2d_Engine.Rendering.RenderPipeline;

namespace Custom2d_Engine.Tilemap
{
    public class ChunkRenderData : IDisposable
    {
        public VertexBufferBinding Binding => new VertexBufferBinding(tilesBuffer, 0, 1);
        private VertexBuffer tilesBuffer;
        private Chunk<InstanceSpriteData> chunk;

        public ChunkRenderData(Chunk<InstanceSpriteData> chunk, RenderPipeline pipeline)
        {
            this.chunk = chunk;
            var size = chunk.ChunkData.Length;
            tilesBuffer = new VertexBuffer(pipeline.Graphics, InstanceSpriteData.VertexDeclaration, size, BufferUsage.WriteOnly);
        }

        public void Flush()
        {
            tilesBuffer.SetData(chunk.ChunkData);
        }

        public void Dispose()
        {
            tilesBuffer?.Dispose();
        }
    }
}

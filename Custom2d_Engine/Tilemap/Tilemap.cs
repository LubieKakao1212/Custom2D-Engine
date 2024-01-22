using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Custom2d_Engine.Tilemap
{
    using Util;
    using Math;

    public class Tilemap<T>
    {
        public IEnumerable<KeyValuePair<Point, Chunk<T>>> Chunks => chunks;

        private Dictionary<Point, Chunk<T>> chunks = new Dictionary<Point, Chunk<T>>();

        public Tilemap()
        {
            
        }

        public void SetTile(Point pos, T tile)
        {
            var chunk = GetChunkAt(pos, true);
            var posInChunk = GridToPosInChunk(pos);

            chunk.SetTile(posInChunk, tile);
        }

        public void SetTilesBlock(Rectangle rect, ReadOnlySpan<T> instance)
        {
            //Todo implement
            throw new NotImplementedException("TODO, Not yet implemented");
        }

        public Chunk<T> GetChunkAt(Point pos, bool createNew)
        {
            return GetChunk(GridToChunkPos(pos), createNew);
        }

        public IEnumerable<Chunk<T>> GetChunksAt(Rectangle rect, bool createNew)
        {
            var c1 = GridToChunkPos(new Point(rect.X, rect.Y));
            var c2 = GridToChunkPos(new Point(rect.X + rect.Width, rect.Y + rect.Height));

            for(var x = c1.X; x <= c2.X; x++)
                for (var y = c1.Y; y <= c2.Y; y++)
                {
                    Chunk<T> chunk = GetChunk(new Point(x, y), createNew);
                    if (chunk != null)
                    {
                        yield return chunk;
                    }
                }
        }

        public Chunk<T> GetChunk(Point chunkPos, bool createNew)
        {
            return createNew ? chunks.GetOrSetToDefaultLazy(chunkPos, (cPos) => new Chunk<T>(cPos)) : chunks.GetValueOrDefault(chunkPos);
        }

        public static Point GridToPosInChunk(Point gridPos)
        {
            return new Point(gridPos.X & Chunk<T>.chunkSizeMask, gridPos.Y & Chunk<T>.chunkSizeMask);
        }

        public static Point GridToChunkPos(Point gridPos)
        {
            return gridPos.FloorDiv(Chunk<T>.chunkSize);
        }

        public static Point ChunkToGridPos(Point chunkPos)
        {
            return new Point(chunkPos.X * Chunk<T>.chunkSize, chunkPos.Y * Chunk<T>.chunkSize);
        }

    }
}

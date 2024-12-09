using System;
using System.Collections.Generic;
using Custom2d_Engine.Math;
using Custom2d_Engine.Util;
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Tilemap {
    public class Tilemap {
        public IEnumerable<KeyValuePair<Point, Chunk>> Chunks => _chunks;

        private readonly Dictionary<Point, Chunk> _chunks = new();

        public void SetTile(Point pos, TileInstance tile) {
            var chunk = GetChunkAt(pos, true);
            var posInChunk = GridToPosInChunk(pos);

            chunk!.SetTile(posInChunk, tile);
        }

        public void SetTilesBlock(Rectangle rect, ReadOnlySpan<TileInstance> instance) {
            //Todo implement
            throw new NotImplementedException("TODO, Not yet implemented");
        }
        
        public Chunk? GetChunkAt(Point pos, bool createNew) {
            return GetChunk(GridToChunkPos(pos), createNew);
        }

        
        public IEnumerable<Chunk> GetChunksAt(Rectangle rect, bool createNew) {
            var c1 = GridToChunkPos(new Point(rect.X, rect.Y));
            var c2 = GridToChunkPos(new Point(rect.X + rect.Width, rect.Y + rect.Height));

            for (var x = c1.X; x <= c2.X; x++)
            for (var y = c1.Y; y <= c2.Y; y++) {
                Chunk? chunk = GetChunk(new Point(x, y), createNew);
                if (chunk != null) {
                    yield return chunk;
                }
            }
        }
        
        public Chunk? GetChunk(Point chunkPos, bool createNew) {
            return createNew
                ? _chunks.GetOrSetToDefaultLazy(chunkPos, (cPos) => new Chunk(cPos))
                : _chunks.GetValueOrDefault(chunkPos);
        }

        public static Point GridToPosInChunk(Point gridPos) {
            return new Point(gridPos.X & Chunk.ChunkSizeMask, gridPos.Y & Chunk.ChunkSizeMask);
        }

        public static Point GridToChunkPos(Point gridPos) {
            return gridPos.FloorDiv(Chunk.ChunkSize);
        }

        public static Point ChunkToGridPos(Point chunkPos) {
            return new Point(chunkPos.X * Chunk.ChunkSize, chunkPos.Y * Chunk.ChunkSize);
        }
    }
}
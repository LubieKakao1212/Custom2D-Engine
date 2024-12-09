using Microsoft.Xna.Framework;
using System;

namespace Custom2d_Engine.Tilemap {
    /// <summary>
    /// Represents a tilemap chunk
    /// </summary>
    public class Chunk {
        public const int ChunkSize = 32;
        public const int ChunkSizeMask = 31;
        public const int TileCount = ChunkSize * ChunkSize;

        public Point ChunkPos { get; private init; }

        public TileInstance[] ChunkData => _chunkData;

        private readonly TileInstance[] _chunkData;

        public Chunk(Point chunkPos) {
            _chunkData = new TileInstance[TileCount];
            ChunkPos = chunkPos;
        }

        public TileInstance GetTile(Point pos) {
            return _chunkData[PosToIndex(pos)];
        }

        public void SetTile(Point pos, TileInstance tile) {
            _chunkData[PosToIndex(pos)] = tile;
        }

        /// <summary>
        /// Sets a rectangle of tiles in chunk
        /// Much faster than multiple <see cref="Chunk.SetTile(Point, TileInstance)"/> calls
        /// Does perform bounds and data size checks
        /// </summary>
        public void SetTilesRect(Rectangle area, in ReadOnlySpan<TileInstance> data) {
            var x = area.X;
            var y = area.Y;
            var width = area.Width;
            var height = area.Height;

            if (x < 0 || (x + width > ChunkSize) || y < 0 || (y + height > ChunkSize)) {
                throw new IndexOutOfRangeException("Outside of chunk");
            }

            if (data.Length < width * height) {
                throw new ArgumentException("Not enough tiles for desired area");
            }

            //Special case, we can do faster
            if (width == ChunkSize) {
                SetSliceUnsafe(y * ChunkSize, data);
                return;
            }

            for (int i = 0; i < area.Y; i++) {
                var slice = data.Slice(i * width, width);

                SetSliceUnsafe(new Point(x, y + i), slice);
            }
        }

        /// <summary>
        /// Sets a horizontal slice of tiles to this chunk <br/>
        /// Much faster than multiple <see cref="Chunk.SetTile(Point, TileInstance)"/> calls <br/>
        /// DOES NOT perform any bounds checks
        /// </summary>
        /// <param name="start"></param>
        /// <param name="slice"></param>
        internal void SetSliceUnsafe(Point start, in ReadOnlySpan<TileInstance> slice) {
            SetSliceUnsafe(PosToIndex(start), slice);
        }

        /// <summary>
        /// Sets a horizontal slice of tiles to this chunk <br/>
        /// Much faster than multiple <see cref="Chunk.SetTile(Point, TileInstance)"/> calls <br/>
        /// DOES NOT perform any bounds checks
        /// </summary>
        /// <param name="startIdx"></param>
        /// <param name="slice"></param>
        internal void SetSliceUnsafe(int startIdx, in ReadOnlySpan<TileInstance> slice) {
            var dstSpan = new Span<TileInstance>(_chunkData, startIdx, slice.Length);
            slice.CopyTo(dstSpan);
        }


        public static int PosToIndex(Point pos) {
            return pos.Y * ChunkSize + pos.X;
        }

        public static Point IndexToPos(int index) {
            return new Point(index & ChunkSizeMask, index / ChunkSize);
        }
    }
}
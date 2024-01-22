using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom2d_Engine.Rendering.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Tilemap
{
    /// <summary>
    /// Represents a tilemap chunk
    /// </summary>
    public class Chunk<T>
    {
        public const int chunkSize = 64;
        public const int chunkSizeMask = 63;
        public const int tileCount = chunkSize * chunkSize;

        public Point ChunkPos { get; private init; }

        /// <summary>
        /// Not null only possible when using <see cref="Chunk{T}"/> with <typeparamref name="T"/> = <see cref="Rendering.RenderPipeline.InstanceSpriteData"/>, There should be a better Place to put this
        /// </summary>
        internal ChunkRenderData RenderData { get; set; }

        public T[] ChunkData => chunkData;

        private T[] chunkData;

        public Chunk(Point chunkPos)
        {
            chunkData = new T[tileCount];
            ChunkPos = chunkPos;
        }

        public T GetTile(Point pos)
        {
            return chunkData[PosToIndex(pos)];
        }

        public void SetTile(Point pos, T tile)
        {
            chunkData[PosToIndex(pos)] = tile;
        }

        /// <summary>
        /// Sets a rectangle of tiles in chunk
        /// Much faster than multiple <see cref="Chunk.SetTile(Point, Sprite)"/> calls
        /// Does perform bounds and data size checks
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="slice"></param>
        public void SetTilesRect(Rectangle area, in ReadOnlySpan<T> data)
        {
            var x = area.X;
            var y = area.Y;
            var width = area.Width;
            var height = area.Height;

            if (x < 0 || (x + width > chunkSize) || y < 0 || (y + height > chunkSize))
            {
                throw new IndexOutOfRangeException("Outside of chunk");
            }

            if (data.Length < width * height)
            {
                throw new ArgumentException("Not enough tiles for desired area");
            }

            //Special case, we can do faster
            if (width == chunkSize)
            {
                SetSliceUnsafe(y * chunkSize, data);
                return;
            }

            for (int i = 0; i < area.Y; i++)
            {
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
        /// <param name="count"></param>
        /// <param name="slice"></param>
        internal void SetSliceUnsafe(Point start, in ReadOnlySpan<T> slice)
        {
            SetSliceUnsafe(PosToIndex(start), slice);
        }

        /// <inheritdoc cref="SetSliceUnsafe(Point, in ReadOnlySpan{TileInstance})"/>
        internal void SetSliceUnsafe(int startIdx, in ReadOnlySpan<T> slice)
        {
            var dstSpan = new Span<T>(chunkData, startIdx, slice.Length);
            slice.CopyTo(dstSpan);
        }

        public static int PosToIndex(Point pos)
        {
            return pos.Y * chunkSize + pos.X;
        }

        public static Point IndexToPos(int index)
        {
            return new Point(index & chunkSizeMask, index / chunkSize);
        }
    }
}

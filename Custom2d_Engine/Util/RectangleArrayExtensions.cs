using Microsoft.Xna.Framework;
using System;

namespace Custom2d_Engine.Util
{
    public static class RectangleArrayExtensions
    {
        public static void SetRectUnchecked<T>(this T[] destinationArray, int arrayWidth, T[] source, Rectangle destinationRect)
        {
            var w = destinationRect.Width;
            var h = destinationRect.Height;
            var targetX = destinationRect.X;
            var targetY = destinationRect.Y;

            for (int y = 0; y < h; y++)
            {
                var sourceIdx = y * w;
                var destinationIdx = (targetY + y) * arrayWidth + targetX;
                var sourceSpan = new Span<T>(source, sourceIdx, w);
                var destinationSpan = new Span<T>(destinationArray, destinationIdx, w);

                sourceSpan.CopyTo(destinationSpan);
            }
        }

        public static void SetRectUnchecked3d<T>(this T[] destinationArray, int arrayWidth, int arrayHeight, T[] source, Rectangle destinationRect, int depth)
        {
            var w = destinationRect.Width;
            var h = destinationRect.Height;
            var targetX = destinationRect.X;
            var targetY = destinationRect.Y;

            for (int y = 0; y < h; y++)
            {
                var sourceIdx = y * w;
                var destinationIdx = ((depth * arrayHeight) + targetY + y) * arrayWidth + targetX;
                var sourceSpan = new Span<T>(source, sourceIdx, w);
                var destinationSpan = new Span<T>(destinationArray, destinationIdx, w);

                sourceSpan.CopyTo(destinationSpan);
            }
        }
    }
}

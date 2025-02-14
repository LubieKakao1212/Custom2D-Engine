using Microsoft.Xna.Framework;
using System;

namespace Custom2d_Engine.Util {
    public static class RectangleArrayExtensions {
        public static void SetRectUnchecked<T>(this T[] destinationArray, int arrayWidth, T[] source,
            Rectangle destinationRect) {
            var w = destinationRect.Width;
            var h = destinationRect.Height;
            var targetX = destinationRect.X;
            var targetY = destinationRect.Y;

            for (int y = 0; y < h; y++) {
                var sourceIdx = y * w;
                var destinationIdx = (targetY + y) * arrayWidth + targetX;
                var sourceSpan = new Span<T>(source, sourceIdx, w);
                var destinationSpan = new Span<T>(destinationArray, destinationIdx, w);

                sourceSpan.CopyTo(destinationSpan);
            }
        }

        public static void SetRectUnchecked3d<T>(this T[] destinationArray, int arrayWidth, int arrayHeight, T[] source,
            Rectangle destinationRect, int depth) {
            var w = destinationRect.Width;
            var h = destinationRect.Height;
            var targetX = destinationRect.X;
            var targetY = destinationRect.Y;

            for (int y = 0; y < h; y++) {
                var sourceIdx = y * w;
                var destinationIdx = ((depth * arrayHeight) + targetY + y) * arrayWidth + targetX;
                var sourceSpan = new Span<T>(source, sourceIdx, w);
                var destinationSpan = new Span<T>(destinationArray, destinationIdx, w);

                sourceSpan.CopyTo(destinationSpan);
            }
        }

        public static void FlipYUnchecked2d<T>(this T[] array2d, int arrayWidth, int arrayHeight) {
            var ySize = arrayHeight / 2; //Rounded down, for odd heights we do not need to swap the bottom row
            var buffer = new T[arrayWidth];
            var bufferSpan = new Span<T>(buffer);
            
            for (int y = 0; y < ySize; y++) {
                var idx1 = y * arrayWidth;
                var idx2 = (arrayHeight - y - 1) * arrayWidth;
                var span1 = new Span<T>(array2d, idx1, arrayWidth);
                var span2 = new Span<T>(array2d, idx2, arrayWidth);

                span1.CopyTo(bufferSpan);
                span2.CopyTo(span1);
                bufferSpan.CopyTo(span2);
            }
        }
    }
}
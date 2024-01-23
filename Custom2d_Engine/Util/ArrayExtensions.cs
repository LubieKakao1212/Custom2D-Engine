using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Custom2d_Engine.Util
{
    public static class ArrayExtensions
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
                //Temporary fix to flipped sprites
                var sourceIdx = y * w;
                var y1 = w - y - 1;
                var destinationIdx = ((depth * arrayHeight) + targetY + y1) * arrayWidth + targetX;
                var sourceSpan = new Span<T>(source, sourceIdx, w);
                var destinationSpan = new Span<T>(destinationArray, destinationIdx, w);

                sourceSpan.CopyTo(destinationSpan);
            }
        }

        public static T[] Fill<T>(this T[] arr, Func<T> filler)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = filler();
            }
            return arr;
        }

        public static T[][] CreateMultiArray<T>(int count1, int count2)
        {
            var arr = new T[count1][];
            for (int i = 0; i < count1; i++)
            {
                arr[i] = new T[count2];
            }

            return arr;
        }
    }
}

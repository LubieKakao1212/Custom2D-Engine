using Microsoft.Xna.Framework;
using System;
using static Microsoft.Xna.Framework.MathHelper;

namespace Custom2d_Engine.Math
{
    public static class MathUtil
    {
        public const float epsilon = 1f / 4096f;

        /// <summary>
        /// Performs a integer division rounding down
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">Bust be positive</param>
        public static int FloorDiv(int a, int b)
        {
            return (a - (a >> 31)) / b + (a >> 31);
        }

        public static float Loop(float t, float period)
        {
            return t - MathF.Floor(t / period) * period;
        }

        public static float LoopAngle(float t)
        {
            return Loop(t, MathHelper.TwoPi);
        }

        public static float LoopedDistance(float a, float b, float period)
        {
            a = Loop(a, period);
            b = Loop(b, period);
            float d1 = MathF.Abs(a - b);
            float d2 = MathF.Abs(a + period - b);
            float d3 = MathF.Abs(a - period - b);
            return Min(Min(d1, d2), d3);
        }

        public static float AngleDistance(float a, float b)
        {
            return LoopedDistance(a, b, TwoPi);
        }

        //TODO Optimise
        public static Vector2 SlerpDirection(Vector2 a, Vector2 b, float t)
        {
            var alpha = a.AngleTo(b);
            var sinAlpha = MathF.Sin(alpha);
            var sinAlphaInv = 1f / sinAlpha;
            var sin1TAlpha = MathF.Sin((1 - t) * alpha);
            var sinTAlpha = MathF.Sin(t * alpha);

            return a * (sin1TAlpha * sinAlphaInv) + b * (sinTAlpha * sinAlphaInv);
        }

        //TODO Optimise
        public static Vector2 SphericalStepDirection(Vector2 a, Vector2 b, float theta)
        {
            var angle = a.AngleTo(b);

            if (angle < theta || angle < epsilon)
            {
                return b;
            }
            return SlerpDirection(a, b, theta / angle);
        }
    }
}

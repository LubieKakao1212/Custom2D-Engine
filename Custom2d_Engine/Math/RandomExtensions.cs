using System;
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Math {
    public static class RandomExtensions {
        public static float RandomNormalised(this Random random) {
            return (random.NextSingle() * 2f) - 1f;
        }

        public static Color RandomColor(this Random random) {
            return new Color(
                random.NextSingle(),
                random.NextSingle(),
                random.NextSingle(),
                1f
            );
        }
    }
}
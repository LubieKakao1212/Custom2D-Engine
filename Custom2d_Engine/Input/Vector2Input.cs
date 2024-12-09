using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Input {
    public class Vector2Input : ContinuousInputBase<Vector2> {
        public Vector2Input(string name) : base(name, Epsilon * Epsilon) {
        }

        protected override float GetDistance(Vector2 value) {
            return value.LengthSquared();
        }
    }
}
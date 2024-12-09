using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Math {
    public struct BoundingRect : IEquatable<BoundingRect> {
        public static BoundingRect Normal { get; } = new BoundingRect(new Vector2(-1, -1), new Vector2(2, 2));

        public Vector2 Min => new Vector2(x, y);
        public Vector2 Max => new Vector2(x + width, y + height);
        public Vector4 Flat => new Vector4(x, y, width, height);

        public float x;
        public float y;
        public float width;
        public float height;

        public BoundingRect(Vector2 pos, Vector2 size) {
            x = pos.X;
            y = pos.Y;

            width = size.X;
            height = size.Y;
        }

        public BoundingRect(float y) {
            this.y = y;
        }

        public void Inflate(float amount) {
            x -= amount;
            y -= amount;

            width += 2 * amount;
            height += 2 * amount;
        }

        public void Inflate(Vector2 amount) {
            x -= amount.X;
            y -= amount.Y;

            width += 2 * amount.X;
            height += 2 * amount.Y;
        }

        public void Scale(float amount) {
            x = -((width / 2f) * amount) - (x + width / 2f);
            y = -((height / 2f) * amount) - (y + height / 2f);

            width *= amount;
            height *= amount;
        }

        public BoundingRect Scaled(float amount) {
            var rect = this;
            rect.Scale(amount);
            return rect;
        }

        public void Transform(in TransformMatrix mat) {
            var min = Min;
            var max = Max;

            var rectPos = mat.T;

            var rectMin = rectPos;
            var rectMax = rectPos;

            for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++) {
                float a = mat[i, j] * min.Get(j);
                float b = mat[i, j] * max.Get(j);
                var minI = rectMin.Get(i);
                var maxI = rectMax.Get(i);
                rectMin.Set(i, minI + (a < b ? a : b));
                rectMax.Set(i, maxI + (a < b ? b : a));
            }

            x = rectMin.X;
            y = rectMin.Y;

            width = rectMax.X - rectMin.X;
            height = rectMax.Y - rectMin.Y;
        }

        public BoundingRect Transformed(in TransformMatrix mat) {
            var br = this;
            br.Transform(mat);
            return br;
        }

        public bool Intersects(in BoundingRect other) {
            var min1 = Min;
            var max1 = Max;

            var min2 = other.Min;
            var max2 = other.Max;

            return
                min2.X < max1.X && min1.X < max2.X &&
                min2.Y < max1.Y && min1.Y < max2.Y;
        }

        public Rectangle ToInt() {
            return new Rectangle(
                (int)MathF.Floor(x),
                (int)MathF.Floor(y),
                (int)MathF.Ceiling(width),
                (int)MathF.Ceiling(height)
            );
        }

        //TODO do epsilon comparison
        public bool Equals(BoundingRect other) {
            if (x == other.x && y == other.y && width == other.width) {
                return height == other.height;
            }

            return false;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            return obj is BoundingRect rect && rect.Equals(this);
        }

        public override int GetHashCode() {
            return HashCode.Combine(x.GetHashCode(), y.GetHashCode(), width.GetHashCode(), height.GetHashCode());
        }

        public static BoundingRect MinMaxRect(Vector2 min, Vector2 max) {
            return new BoundingRect(min, max - min);
        }
    }
}
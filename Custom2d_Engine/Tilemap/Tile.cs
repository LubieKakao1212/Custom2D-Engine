using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering.Sprites;

namespace Custom2d_Engine.Tilemap {
    public class Tile {
        public Sprite Sprite { get; init; } = new Sprite();

        //private Matrix2x2 transform;
        public float Order { get; set; }

        public Color Tint { get; set; }

        public TransformMatrix Transform { get; set; } = TransformMatrix.Identity;
    }

    public readonly struct TileInstance {
        public Tile? Tile { get; init; }
        public Matrix2x2 Transform { get; init; }

        public TileInstance() : this(null) {
        }

        public TileInstance(Tile? tile) {
            Transform = new Matrix2x2(1f);
            Tile = tile;
        }

        public TileInstance(Tile? tile, Matrix2x2 transfrom) {
            Tile = tile;
            Transform = transfrom;
        }

        public static implicit operator TileInstance(Tile tile) {
            return new TileInstance(tile, new Matrix2x2(1f));
        }
    }
}
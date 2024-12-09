using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Custom2d_Engine.Rendering {
    public static class Effects {
        [NotNull] public static Effect? TilemapDefault { get; private set; }
        [NotNull] public static Effect? Default { get; private set; }

        //TODO refactor to RSS
        public const string CameraRS = "CameraRS";
        public const string CameraT = "CameraT";

        public const string ObjRSS = "ObjRSS";
        public const string ObjT = "ObjT";

        //TODO refactor to RSS
        public const string GridRS = "GridRS";
        public const string GridT = "GridT";

        //dafuq???
        public const string SpriteAtlas = "AtlasSampler+SpriteAtlas";
        public const string AtlasSize = "AtlasSize";

        private const string TilemapDefaultEffectPath = "Tilemap";
        private const string DefaultEffectPath = "Default";

        public static void Init(ContentManager content) {
            TilemapDefault = content.Load<Effect>(TilemapDefaultEffectPath);
            Default = content.Load<Effect>(DefaultEffectPath);
        }
    }
}
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering
{
    public static class Effects
    {
        public static Effect TilemapDefault { get; private set; }
        public static Effect Default { get; private set; }
        public static Effect RawTex { get; private set; }

        //TODO refactor to RSS
        public const string CameraRS = "CameraRS";
        public const string CameraT = "CameraT";

        public const string ObjRSS = "ObjRSS";
        public const string ObjT = "ObjT";

        //TODO refactor to RSS
        public const string GridRS = "GridRS";
        public const string GridT = "GridT";

        internal const string Tex = "Tex";

        public const string AtlasSize = "AtlasSize";
        //dafuq???
        //public const string SpriteAtlas = "AtlasSampler+SpriteAtlas";
        public const string ColorAtlas = "ColorAtlas";
        public const string NormalAtlas = "ColorAtlas";
        public const string EmissionAtlas = "EmissionAtlas";

        public const string SceneLights = "SceneLights";

        private const string TilemapDefaultEffectPath = "Tilemap";
        private const string DefaultEffectPath = "Default";
        private const string RawTexPath = "RawTex";
        
        public static void Init(ContentManager content)
        {
            TilemapDefault = content.Load<Effect>(TilemapDefaultEffectPath);
            Default = content.Load<Effect>(DefaultEffectPath);
            RawTex = content.Load<Effect>(RawTexPath);
        }
    }
}

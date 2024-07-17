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

        public const string RotScale = "RotScale";
        public const string Pos = "Pos";
        
        //TODO refactor to RSS
        public const string ChunkRS = "ChunkRS";
        public const string ChunkT = "ChunkT";
        public const string Spacing = "Spacing";

        internal const string Tex = "Tex";

        public const string AtlasSize = "AtlasSize";
        //dafuq???
        //public const string SpriteAtlas = "AtlasSampler+SpriteAtlas";
        public const string ColorAtlas = "AtlasSampler+ColorAtlas";
        public const string NormalAtlas = "AtlasSampler+NormalAtlas";
        public const string EmissionAtlas = "AtlasSampler+EmissionAtlas";

        public const string SceneLights = "SceneLights";
        public const string SceneNormals = "SceneNormals";

        private const string TilemapDefaultEffectPath = "Tilemap";
        private const string DefaultEffectPath = "Default";
        private const string RawTexPath = "RawTex";
        
        public static void Init(ContentManager content)
        {
            TilemapDefault = content.Load<Effect>(TilemapDefaultEffectPath);
            Default = content.Load<Effect>(DefaultEffectPath);
            RawTex = content.Load<Effect>(RawTexPath);

            Lights.GlobalLight = content.Load<Effect>(Lights.GlobalLightPath);
            Lights.PointLight = content.Load<Effect>(Lights.PointLightPath);
        }

        public static class Lights
        {
            public static Effect GlobalLight { get; internal set; }
            public static Effect PointLight { get; internal set; }

            public const string Intensity = "Intensity";
            public const string Tint = "Tint";
            public const string Height = "Height";
            public const string Direction = "Direction";
            public const string InnerRadiusRatio = "InnerRadiusRatio";
            public const string OutInAngleRatio = "OutInAngleRatio";
            public const string ObjWorldPos = "ObjWorldPos";


            internal const string GlobalLightPath = "Lights/GlobalLight";
            internal const string PointLightPath = "Lights/PointLight";
        }
    }
}

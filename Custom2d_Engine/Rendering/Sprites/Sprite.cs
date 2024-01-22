using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering.Sprites
{
    public class Sprite
    {
        public BoundingRect TextureRect { get; set; }
        public int TextureIndex { get; set; }
        
        public Vector4 AtlasPos => new Vector4(TextureRect.X + TextureIndex, TextureRect.Y, TextureRect.Width, TextureRect.Height);
    }
}

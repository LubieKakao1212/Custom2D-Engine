using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering.Sprites.Atlas
{
    public struct AtlasRegion
    {
        public bool IsValid => sourceTextureIdx >= 0 && sourceRect.Width > 0 && sourceRect.Height > 0;
        
        public int sourceTextureIdx = -1;
        public Rectangle sourceRect;
        public Point destinationPosition;
        public Sprite destinationSprite;

        public AtlasRegion() { }
    }
}

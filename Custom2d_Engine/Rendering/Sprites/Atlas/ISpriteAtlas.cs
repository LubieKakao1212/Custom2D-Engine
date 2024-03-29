﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering.Sprites.Atlas
{
    public interface ISpriteAtlas : IDisposable
    {
        Sprite[] AddTextureRects(Texture2D[] textures, params Rectangle[] regions);

        public void Compact(int maxSize);
    }
}

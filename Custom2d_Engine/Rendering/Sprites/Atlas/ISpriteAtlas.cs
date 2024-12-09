using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Custom2d_Engine.Rendering.Sprites.Atlas {
    public interface ISpriteAtlas : IDisposable {
        Sprite[] AddTextureRects(Texture2D texture, params Rectangle[] regions);

        public void Compact(int maxSize);
    }
}
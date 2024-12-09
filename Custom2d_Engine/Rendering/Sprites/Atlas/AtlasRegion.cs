using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Custom2d_Engine.Rendering.Sprites.Atlas {
    public struct AtlasRegion : IDisposable {
        public bool IsValid => _sourceTexture != null && sourceRect.Width > 0 && sourceRect.Height > 0;

        public Texture2D SourceTexture {
            get => _sourceTexture ?? throw new ObjectDisposedException("AtlasRegion has been disposed");
            init => _sourceTexture = value;
        }

        private Texture2D? _sourceTexture;
        public Rectangle sourceRect;
        public Point destinationPosition;
        public Sprite destinationSprite;

        public void Dispose() {
            _sourceTexture?.Dispose();
            _sourceTexture = null;
        }
    }
}
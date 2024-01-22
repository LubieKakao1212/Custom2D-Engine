using Custom2d_Engine.Rendering.Sprites;
using TiledLib;

namespace Custom2d_Engine.TMX
{
    public class TilesetSprites
    {
        private Tileset tileset;

        private Sprite[] sprites;

        internal TilesetSprites(Tileset tileset, Sprite[] sprites)
        {
            this.tileset = tileset;
            sprites = new Sprite[tileset.TileCount];
        }

        public Sprite GetTileSprite(int id)
        {
            return sprites[id];
        }
    }
}

using Custom2d_Engine.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib;

namespace Custom2d_Engine.TMX
{
    public class TMXCache
    {
        private Dictionary<string, Tileset> tilesetCache = new Dictionary<string, Tileset>();

        private Dictionary<string, Sprite[]> tilesetSpritesCache = new Dictionary<string, Sprite[]>();

        public bool TryGetTileset(string parth, out Tileset tileset)
        {
            return tilesetCache.TryGetValue(parth, out tileset);
        }

        public void AddLoadedTileset(string path, Tileset tileset, Sprite[] sprites)
        {
            tilesetCache.Add(path, tileset);
            tilesetSpritesCache.Add(path, sprites);
        }

        public bool TryGetSprites(string path, out Sprite[] sprites)
        {
            return tilesetSpritesCache.TryGetValue(path, out sprites);
        }
    }
}

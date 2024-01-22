using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Rendering.Sprites.Atlas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiledLib;

namespace Custom2d_Engine.TMX
{
    public class TMXLoader<TPixel> where TPixel : struct
    {
        private ContentManager Content;
        private SpriteAtlasLoader<TPixel> SpriteLoader;

        private Dictionary<string, Tileset> tilesetCache = new Dictionary<string, Tileset>();

        private Dictionary<string, TilesetSprites> tilesetSpritesCache = new Dictionary<string, TilesetSprites>();

        public TMXLoader(ContentManager content, SpriteAtlasLoader<TPixel> spriteLoader) 
        {
            this.Content = content;
            this.SpriteLoader = spriteLoader;
        }

        public Map LoadMap(string filename)
        {
            filename = Path.Combine(Content.RootDirectory, filename);
            var path = Path.GetDirectoryName(filename);
            using (var stream = File.OpenRead(filename))
            {
                using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 1024, leaveOpen: true);
                var map = streamReader.ReadTmxMap();

                ITileset[] tilesets = map.Tilesets;
                foreach (ITileset tileset in tilesets)
                {
                    if (!(tileset is ExternalTileset externalTileset))
                    {
                        continue;
                    }

                    externalTileset.LoadTileset(delegate (ExternalTileset e)
                    {
                        e.Source = GetContentPath(Path.Combine(path, e.Source));
                        return LoadTileset(e.Source);
                    });
                }

                foreach (var ts in map.Tilesets.OfType<ExternalTileset>())
                    ts.LoadTileset();

                return map;
            }
        }

        public Tileset LoadTileset(string filename)
        {
            if (!tilesetCache.TryGetValue(filename, out var tileset))
            {
                tileset = Tileset.FromStream(File.OpenRead(GetLoadingPath(filename)));
            }
            return tileset;
        }

        public Sprite[] LoadTilesetSprites(ITileset source)
        {
            var spritePath = source.Properties[TMXProperties.Tileset.Sprite];

            var w = source.TileWidth;
            var h = source.TileHeight;

            var rects = new Rectangle[source.TileCount];

            //TODO Probably Y is inverted
            for (int x = 0; x < source.Columns; x++)
                for (int y = 0; y < source.Rows; y++)
                {
                    rects[y * source.Columns + x] = new Rectangle(x * w, y * h, w, h);
                }

            return SpriteLoader.Load(spritePath, rects);
        }



        private string GetContentPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Content.RootDirectory, path);
            }
            return Path.GetRelativePath(Content.RootDirectory, path);//Path.IsPathRooted(path) ? path : Path.Combine(Path.GetDirectoryName(filename), path);
        }

        private string GetLoadingPath(string path)
        {
            return Path.Combine(Content.RootDirectory, path);
        }
    }

    public static class TMXProperties
    {
        public static class Tileset
        {
            public const string Sprite = "Sprite";
        }
    }
}

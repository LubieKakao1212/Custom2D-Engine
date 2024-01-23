using Custom2d_Engine.Rendering.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledLib;
using TiledLib.Layer;
using TiledLib.Objects;

namespace Custom2d_Engine.TMX
{
    public class LoadedMap<TPixel> where TPixel : struct
    {
        public delegate void TileLayerHandler(LoadedMap<TPixel> map, Point position, int gid);
        public delegate void ObjectLayerHandler(LoadedMap<TPixel> map, BaseObject obj);

        public int Width => map.Width;
        public int Height => map.Height;

        private Map map;
        private Dictionary<string, BaseLayer> layerCache = new Dictionary<string, BaseLayer>();
        private Dictionary<Tileset, Sprite[]> embeddedTilesetsSpriteCache = new Dictionary<Tileset, Sprite[]>();
        private TMXCache tmxCache;

        internal LoadedMap(Map map, TMXCache tmxCache, TMXLoader<TPixel> loader)
        {
            this.map = map;
            this.tmxCache = tmxCache;
            SetupLayerCace();
            LoadTilesets(loader);
        }

        public void ProcessTileLayer(string layerName, TileLayerHandler tileHandler)
        {
            var layer = layerCache[layerName];
            if (layer is not TileLayer tileLayer)
            {
                throw new Exception($"Layer {layerName} is not a tile layer");
            }

            var w = map.Width;
            var h = map.Height;

            for (int i = 0; i < tileLayer.Data.Length; i++)
            {
                tileHandler(this, new Point(i % w, h - (i / w)), tileLayer.Data[i]);
            }
        }

        public void ProcessObjectLayer(string layerName, ObjectLayerHandler objectHandler)
        {
            var layer = layerCache[layerName];
            if (layer is not ObjectLayer objLayer)
            {
                throw new Exception($"Layer {layerName} is not an object layer");
            }

            for (int i = 0; i < objLayer.Objects.Length; i++)
            {
                objectHandler(this, objLayer.Objects[i]);
            }
        }

        public ITileset GetTileset(int gid)
        {
            var tileset = map.Tilesets.Where((tileset) => tileset.FirstGid <= gid && tileset.FirstGid + tileset.TileCount > gid).Single();
            return tileset;
        }

        public Sprite GetTileSprite(int gid)
        {
            var tileset = GetTileset(gid);
            if (tileset is Tileset ts)
            {
                return embeddedTilesetsSpriteCache[ts][gid - ts.FirstGid];
            }
            else if (tileset is ExternalTileset ets)
            {
                if (tmxCache.TryGetSprites(ets.Source, out var sprites))
                {
                    return sprites[gid - ets.FirstGid];
                }
                throw new Exception($"Invalid {nameof(ExternalTileset)}");
            }
            throw new Exception($"Invalid {nameof(ITileset)} Type");
        }

        public void LoadTilesets(TMXLoader<TPixel> loader)
        {
            foreach (var tileset in map.Tilesets)
            {
                if (tileset is not Tileset ts)
                {
                    continue;
                }
                embeddedTilesetsSpriteCache.Add(ts, loader.LoadTilesetSprites(tileset));
            }
        }

        private void SetupLayerCace()
        {
            foreach (var layer in map.Layers)
            {
                layerCache.Add(layer.Name, layer);
            }
        }
    }
}

using Custom2d_Engine.Rendering;
using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Tilemap;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib.Layer;
using TiledLib.Objects;
using static Custom2d_Engine.Rendering.RenderPipeline;

namespace Custom2d_Engine.TMX.LayerProcessor
{
    public static class TileLayerProcessors<TPixel> where TPixel : struct
    {
        public static LoadedMap<TPixel>.TileLayerHandler FillTilemap(Tilemap<InstanceSpriteData> tilemap, Point offset, NullTileHandling nullHandling)
        {
            return (map, position, gid) =>
            {
                var isd = new InstanceSpriteData();
                if (gid == 0)
                {
                    switch (nullHandling)
                    {
                        case NullTileHandling.Skip:
                            return;
                        case NullTileHandling.Empty:
                            isd.atlasPos = Sprite.Empty.AtlasPos;
                            break;
                    }
                }
                else
                {
                    isd.atlasPos = map.GetTileSprite(gid).AtlasPos;
                }
                isd.color = Color.White.ToVector4();
                tilemap.SetTile(position + offset, isd);
            };
        }

    }

    public enum NullTileHandling
    {
        Skip = 0,
        Empty = 1
    }
}

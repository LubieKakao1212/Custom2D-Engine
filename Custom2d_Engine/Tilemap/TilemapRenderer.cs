﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering;
using Custom2d_Engine.Scenes;
using Custom2d_Engine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Custom2d_Engine.Tilemap
{
    [Obsolete("Broken, Rewrite Needed")]
    public class TilemapRenderer : SpecialRenderedObject
    {
        private static VertexDeclaration TileInstanceDeclaration { get; } = new VertexDeclaration(
            
            new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1), //2x2 RotScale
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.Position, 2), //2 Position

            new VertexElement(sizeof(float) * (4 + 2), VertexElementFormat.Vector4, VertexElementUsage.Position, 3), //2x2 TileRotScale
            new VertexElement(sizeof(float) * (4 + 2 + 4), VertexElementFormat.Vector2, VertexElementUsage.Position, 4), //2 TileOffset
            
            new VertexElement(sizeof(float) * (4 + 2 + 4 + 2), VertexElementFormat.Vector4, VertexElementUsage.Color, 0), //4 Tint
            
            new VertexElement(sizeof(float) * (4 + 2 + 4 + 2 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0) //[2, 2] Texture Rext
            );

        private Tilemap tilemap;
        private Grid grid;

        private DynamicVertexBuffer instanceBuffer;

        #region Preallocations
        private TileInstanceRenderData[] renderDataArray = new TileInstanceRenderData[RenderPipeline.MaxInstanceCount];
        private List<Ordered<TileInstanceRenderData>> ordersList = new(RenderPipeline.MaxInstanceCount);
        #endregion

        public TilemapRenderer(Tilemap tilemap, Grid grid, RenderPipeline pipeline, Color color, float drawOrder) : base(pipeline, color, drawOrder)
        {
            this.tilemap = tilemap;
            this.grid = grid;
            instanceBuffer = new DynamicVertexBuffer(pipeline.Graphics, TileInstanceDeclaration, RenderPipeline.MaxInstanceCount, BufferUsage.WriteOnly);
        }

        protected override void RenderNormals()
        {
            /*var gridWtL = Matrix2x2.Scale(grid.CellSize).Inverse() * grid.Transform.WorldToLocal;
            var camVtW = camera.ProjectionMatrix.Inverse();

            var VtG = gridWtL * camVtW;

            var rect = Camera.CullingRect.Transformed(VtG);

            var GtV = VtG.Inverse();

            ordersList.Clear();

            foreach (var chunk in tilemap.GetChunksAt(rect.ToInt(), false))
            {
                ////TODO Cache Tiles
                if(ordersList.Capacity - ordersList.Count < Chunk.tileCount)
                {
                    ordersList.Capacity += Chunk.tileCount;
                }

                var chunkPos = chunk.ChunkPos;
                var chunkGridPos = Tilemap.ChunkToGridPos(chunkPos);

                var i = 0;
                
                ordersList.AddRange(chunk.ChunkData.SelectMany((tile) =>
                {
                    i++;
                    if (tile.Tile == null)
                    {
                        return Enumerable.Empty<Ordered<TileInstanceRenderData>>();
                    }
                    var pos = Chunk.IndexToPos(i - 1);
                    var position = pos.ToVector2() + chunkGridPos.ToVector2();
                    position *= grid.CellSize;
                    position += grid.CellSize / 2f;

                    //TODO
                    #region Tmp
                    var tint = tile.Tile.Tint.ToVector4();

                    tint.X *= tint.W;
                    tint.Y *= tint.W;
                    tint.Z *= tint.W;
                    #endregion

                    return Enumerable.Repeat(new Ordered<TileInstanceRenderData>() { Value = new TileInstanceRenderData()
                    {
                        RotScale = tile.Transform.Flat,
                        Position = position,
                        TileRotScale = tile.Tile.Transform.RS.Flat,
                        TilePosition = tile.Tile.Transform.T,
                        Color = tint,
                        TexCoord = tile.Tile.Sprite.TextureRect.Flat
                    }, Order = tile.Tile.Order }
                    , 1);
                }));
            }

            var effect = Effects.TilemapDefault;

            effect.CurrentTechnique = effect.Techniques["Unlit"];
            effect.Parameters[Effects.GridRS].SetValue(grid.Transform.LocalToWorld.RS.Flat);
            effect.Parameters[Effects.GridT].SetValue(grid.Transform.LocalToWorld.T);

            using var effectScope = new RenderPipeline.EffectScope(Pipeline, effect);
            using var cameraScope = new RenderPipeline.CameraScope(Pipeline, GtV);

            Pipeline.Rendering.DrawSortedLayerQuads(instanceBuffer, ordersList.ToArray());*/
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TileInstanceRenderData
        {
            public Vector4 RotScale;
            public Vector2 Position;
            
            public Vector4 TileRotScale;
            public Vector2 TilePosition;
            
            public Vector4 Color;
            public Vector4 TexCoord;
        }
    }
}

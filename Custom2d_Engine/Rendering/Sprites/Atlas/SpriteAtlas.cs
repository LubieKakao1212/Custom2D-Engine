using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Math;
using Custom2d_Engine.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Custom2d_Engine.Rendering.Sprites.Atlas
{
    public class SpriteAtlas<T> : ISpriteAtlas where T : struct
    {
        public Texture3D[] AtlasTextures => atlasTextures;
        public GraphicsDevice Graphics => graphics;

        private const int maxSizeInternal = 8192;

        private List<AtlasRegion> regions = new();
        private List<Texture2D>[] sourceTextures;

        private T[] baseColors;
        private Texture3D[] atlasTextures;

        private int size;

        private int textureCount;

        private GraphicsDevice graphics;

        private SurfaceFormat textureFormat;

        private bool compacted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minimumTextureSize">Currently not used, atlas is always full size</param>
        public SpriteAtlas(GraphicsDevice graphics, int minimumTextureSize = maxSizeInternal, int secondaryTextureCount = 0)
        {
            sourceTextures = new List<Texture2D>[secondaryTextureCount + 1].Fill(() => new List<Texture2D>());
            atlasTextures = new Texture3D[secondaryTextureCount + 1];
            baseColors = Enumerable.Repeat(default(T), secondaryTextureCount + 1).ToArray();

            size = minimumTextureSize;
            this.graphics = graphics;
            if (typeof(T) == typeof(Vector4))
            { 
                textureFormat = SurfaceFormat.Vector4;
            }
            else if (typeof(T) == typeof(Color))
            {
                textureFormat = SurfaceFormat.Color;
            }
            else
            {
                throw new ApplicationException($"Data type {typeof(T)} is not supported.");
            }
        }

        public void SetBaseColor(int texIdx, T color)
        {
            baseColors[texIdx] = color;
        }

        public Sprite[] AddTextureRects(Texture2D[] textures, params Rectangle[] rects)
        {
            var output = new Sprite[MathHelper.Max(rects.Length, 1)];

            regions.Capacity += output.Length;

            if (rects.Length == 0)
            {
                var t = textures.Where((t) => t != null).First();
                rects = new Rectangle[] { new Rectangle(0, 0, t.Width, t.Height) };
            }
                       
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new Sprite();
                regions.Add(new AtlasRegion() 
                {
                    sourceTextureIdx = sourceTextures[0].Count,
                    sourceRect = rects[i],
                    destinationSprite = output[i]
                });
                for (int tex = 0; tex < sourceTextures.Length; tex++)
                {
                    sourceTextures[tex].Add(textures[tex]);
                }
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSize">Currently not used, atlas is always full size</param>
        public void Compact(int maxSize = maxSizeInternal)
        {
            if (compacted)
            {
                throw new ApplicationException($"Atlas already compacted");
            }

            textureCount = 1;

            regions.Sort((a, b) => b.sourceRect.Height - a.sourceRect.Height);

            SortedDictionary<int, List<Rectangle>> spaces = new SortedDictionary<int, List<Rectangle>>();

            spaces.AddNested(size, new Rectangle(0, 0, size, size));

            for (int i=0; i<regions.Count; i++)
            {
                var region = regions[i];
                var sourceRect = region.sourceRect;
                if (region.IsValid)
                {
                    if (spaces.Count == 0 || spaces.Last().Value[0].Height < sourceRect.Height)
                    {
                        //TODO Resize And insert
                        //Add txture
                        IncrementTextureCount(ref sourceRect, spaces);
                        continue;
                    }
                    
                    bool flag = true;
                    IEnumerator<List<Rectangle>> spacesEnumerator = spaces.Values.GetEnumerator();

                    while (flag && spacesEnumerator.MoveNext())
                    {
                        List<Rectangle> space = spacesEnumerator.Current;
                        if (space[0].Height < sourceRect.Height)
                        {
                            continue;
                        }

                        var spaceHeight = space[0].Height;
                        var perfectH = spaceHeight == sourceRect.Height;

                        for (int k = 0; k < space.Count; k++)
                        {
                            if (space[k].Width >= sourceRect.Width)
                            {
                                Rectangle rect = sourceRect;
                                rect.X = space[k].X;
                                rect.Y = space[k].Y;
                                sourceRect = rect;
                                if (perfectH && space[k].Width == sourceRect.Width) // remove
                                {
                                    spaces.RemoveNested(spaceHeight, space[k]);
                                }
                                else if (perfectH) //shrink horizontally
                                {
                                    //Don't have to add and remove, height doesn't change
                                    space[k] = new Rectangle(space[k].X + sourceRect.Width, space[k].Y, space[k].Width - sourceRect.Width, space[k].Height);
                                }
                                else if (space[k].Width == sourceRect.Width) //shrink vertically
                                {
                                    Rectangle newSpace = new Rectangle(space[k].X, space[k].Y + sourceRect.Height, space[k].Width, spaceHeight - sourceRect.Height);
                                    spaces.RemoveNested(spaceHeight, space[k]);
                                    spaces.AddNested(newSpace.Height, newSpace);
                                }
                                else
                                {
                                    //Top
                                    var topHeight = spaceHeight - sourceRect.Height;
                                    spaces.AddNested(topHeight, new Rectangle(space[k].X, space[k].Y + sourceRect.Height, sourceRect.Width, topHeight));
                                    //Right
                                    space[k] = new Rectangle(space[k].X + sourceRect.Width, space[k].Y, space[k].Width - sourceRect.Width, spaceHeight);
                                }

                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag)
                    {
                        IncrementTextureCount(ref sourceRect, spaces);
                    }
                }
                
                region.destinationPosition = sourceRect.Location;
                regions[i] = region;
            }

            compacted = true;

            CreateAtlasTextures();
        }

        private void CreateAtlasTextures()
        {
            int atlasCount = atlasTextures.Length;
            for (int i = 0; i< atlasCount; i++)
            {
                atlasTextures[i] = new Texture3D(Graphics, size, size, textureCount, false, textureFormat);
            }

            var texturePixelCount = size * size;

            var atlasPixels = ArrayExtensions.CreateMultiArray<T>(atlasCount, texturePixelCount * textureCount);

            for (int i = 0; i < atlasPixels.Length; i++)
            {
                Array.Fill(atlasPixels[i], baseColors[i]);
            }
            
            foreach (var region in regions)
            {
                var pos = region.destinationPosition;
                var x = pos.X % size;
                var idx = pos.X / size;

                #region Fill Atlas
                for (int i = 0; i < atlasCount; i++)
                {
                    if (sourceTextures[i][region.sourceTextureIdx] != null)
                    {
                        sourceTextures[i][region.sourceTextureIdx].TransferPixels3d(atlasPixels[i], size, size, region.sourceRect, x, pos.Y, idx);
                    }
                }
                #endregion

                #region SetSprite Data

                region.destinationSprite.TextureRect = new BoundingRect(
                    new Vector2((float)x / size, (float)pos.Y / size), 
                    new Vector2(
                        (float)region.sourceRect.Width / size, 
                        (float)region.sourceRect.Height/ size)
                    );
                region.destinationSprite.TextureIndex = idx;

                #endregion
            }

            atlasTextures.SetData(atlasPixels);
        }

        private void IncrementTextureCount(ref Rectangle sourceRect, SortedDictionary<int, List<Rectangle>> spaces)
        {
            textureCount++;
            var newTexIdx = textureCount - 1;
            sourceRect.X = newTexIdx * size;
            sourceRect.Y = 0;
            Rectangle right = new Rectangle(newTexIdx * size + sourceRect.Width, 0, size - sourceRect.Width, size),
                top = new Rectangle(newTexIdx * size, sourceRect.Height, sourceRect.Width, size - sourceRect.Height);

            spaces.AddNested(right.Height, right);
            spaces.AddNested(top.Height, top);
        }

        public void Dispose()
        {
            foreach (var atlas in atlasTextures)
            {
                atlas.Dispose();
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Math;
using Custom2d_Engine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom2d_Engine.Rendering.Sprites.Atlas {
    public class SpriteAtlas<T> : ISpriteAtlas where T : struct {
        public Texture3D? AtlasTextures => _atlasTextures;

        private const int MaxSizeInternal = 8192;

        private readonly List<AtlasRegion> _regions = new();

        private Texture3D? _atlasTextures;

        private readonly int _size;

        private int _textureCount;

        private readonly GraphicsDevice _graphics;

        private readonly SurfaceFormat _textureFormat;

        private bool _compacted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="minimumTextureSize">Currently not used, atlas is always full size</param>
        public SpriteAtlas(GraphicsDevice graphics, int minimumTextureSize = MaxSizeInternal) {
            _size = minimumTextureSize;
            this._graphics = graphics;
            if (typeof(T) == typeof(Vector4)) {
                _textureFormat = SurfaceFormat.Vector4;
            }
            else if (typeof(T) == typeof(Color)) {
                _textureFormat = SurfaceFormat.Color;
            }
            else {
                throw new ApplicationException($"Data type {typeof(T)} is not supported.");
            }
        }

        public Sprite[] AddTextureRects(Texture2D texture, params Rectangle[] rects) {
            var output = new Sprite[MathHelper.Max(rects.Length, 1)];

            _regions.Capacity += output.Length;

            if (rects.Length == 0) {
                output[0] = new Sprite();
                _regions.Add(new AtlasRegion() {
                    SourceTexture = texture,
                    sourceRect = texture.Bounds,
                    destinationSprite = output[0]
                });
            }
            else
                for (int i = 0; i < output.Length; i++) {
                    output[i] = new Sprite();
                    _regions.Add(new AtlasRegion() {
                        SourceTexture = texture,
                        sourceRect = rects[i],
                        destinationSprite = output[i]
                    });
                }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSize">Currently not used, atlas is always full size</param>
        public void Compact(int maxSize = MaxSizeInternal) {
            if (_compacted) {
                throw new ApplicationException($"Atlas already compacted");
            }

            _textureCount = 1;

            _regions.Sort((a, b) => b.sourceRect.Height - a.sourceRect.Height);

            SortedDictionary<int, List<Rectangle>> spaces = new SortedDictionary<int, List<Rectangle>>();

            spaces.AddNested(_size, new Rectangle(0, 0, _size, _size));

            for (int i = 0; i < _regions.Count; i++) {
                var region = _regions[i];
                var sourceRect = region.sourceRect;
                if (region.IsValid) {
                    if (spaces.Count == 0 || spaces.Last().Value[0].Height < sourceRect.Height) {
                        //TODO Resize And insert
                        //Add txture
                        IncrementTextureCount(ref sourceRect, spaces);
                        continue;
                    }

                    bool flag = true;
                    IEnumerator<List<Rectangle>> spacesEnumerator = spaces.Values.GetEnumerator();

                    while (flag && spacesEnumerator.MoveNext()) {
                        List<Rectangle> space = spacesEnumerator.Current;
                        if (space[0].Height < sourceRect.Height) {
                            continue;
                        }

                        var spaceHeight = space[0].Height;
                        var perfectH = spaceHeight == sourceRect.Height;

                        for (int k = 0; k < space.Count; k++) {
                            if (space[k].Width >= sourceRect.Width) {
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
                                    space[k] = new Rectangle(space[k].X + sourceRect.Width, space[k].Y,
                                        space[k].Width - sourceRect.Width, space[k].Height);
                                }
                                else if (space[k].Width == sourceRect.Width) //shrink vertically
                                {
                                    Rectangle newSpace = new Rectangle(space[k].X, space[k].Y + sourceRect.Height,
                                        space[k].Width, spaceHeight - sourceRect.Height);
                                    spaces.RemoveNested(spaceHeight, space[k]);
                                    spaces.AddNested(newSpace.Height, newSpace);
                                }
                                else {
                                    //Top
                                    var topHeight = spaceHeight - sourceRect.Height;
                                    spaces.AddNested(topHeight,
                                        new Rectangle(space[k].X, space[k].Y + sourceRect.Height, sourceRect.Width,
                                            topHeight));
                                    //Right
                                    space[k] = new Rectangle(space[k].X + sourceRect.Width, space[k].Y,
                                        space[k].Width - sourceRect.Width, spaceHeight);
                                }

                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag) {
                        IncrementTextureCount(ref sourceRect, spaces);
                    }
                }

                region.destinationPosition = sourceRect.Location;
                _regions[i] = region;
            }

            _compacted = true;

            CreateAtlasTextures();
        }

        private void CreateAtlasTextures() {
            _atlasTextures = new Texture3D(_graphics, _size, _size, _textureCount, false, _textureFormat);

            var texturePixelCount = _size * _size;

            var atlasPixels = new T[texturePixelCount * _textureCount];

            foreach (var region in _regions) {
                var pos = region.destinationPosition;
                var x = pos.X % _size;
                var idx = pos.X / _size;

                #region Fill Atlas

                var sourceRect = region.sourceRect;
                var rawData = GetTextureData(region.SourceTexture, sourceRect);
                rawData.FlipYUnchecked2d(sourceRect.Width, sourceRect.Height);
                var data = new T[rawData.Length];
                if (data is Vector4[] vArr) {
                    rawData.CopyTo(vArr, 0);
                }
                else if (data is Color[] cArr) {
                    for (int i = 0; i < data.Length; i++) {
                        cArr[i] = new Color(rawData[i]);
                    }
                }

                atlasPixels.SetRectUnchecked3d(_size, _size, data, new Rectangle(
                    x, pos.Y,
                    region.sourceRect.Width, region.sourceRect.Height), idx);

                #endregion

                #region SetSprite Data

                region.destinationSprite.TextureRect = new BoundingRect(
                    new Vector2((float)x / _size, (float)pos.Y / _size),
                    new Vector2(
                        (float)region.sourceRect.Width / _size,
                        (float)region.sourceRect.Height / _size)
                );
                region.destinationSprite.TextureIndex = idx;

                #endregion
            }

            // var fullRect = new Rectangle(0, 0, _size, _size);

            _atlasTextures.SetData(atlasPixels);
        }

        //Can be done better, I think
        private Vector4[] GetTextureData(Texture2D source, Rectangle sourceRect) {
            var dataSize = sourceRect.Width * sourceRect.Height;

            if (source.Format == SurfaceFormat.Color) {
                var pixels = new Color[dataSize];
                source.GetData(0, sourceRect, pixels, 0, dataSize);

                return pixels.Select((p) => p.ToVector4()).ToArray();
            }
            else if (source.Format == SurfaceFormat.Vector4) {
                var pixels = new Vector4[dataSize];
                source.GetData(0, sourceRect, pixels, 0, dataSize);
                return pixels;
            }
            else
                throw new ApplicationException($"Unsuported texture format {source.Format}");
        }

        private void IncrementTextureCount(ref Rectangle sourceRect, SortedDictionary<int, List<Rectangle>> spaces) {
            _textureCount++;
            var newTexIdx = _textureCount - 1;
            sourceRect.X = newTexIdx * _size;
            sourceRect.Y = 0;
            Rectangle right = new Rectangle(newTexIdx * _size + sourceRect.Width, 0, _size - sourceRect.Width, _size),
                top = new Rectangle(newTexIdx * _size, sourceRect.Height, sourceRect.Width, _size - sourceRect.Height);

            spaces.AddNested(right.Height, right);
            spaces.AddNested(top.Height, top);
        }

        public void Dispose() {
            _atlasTextures?.Dispose();
        }
    }
}
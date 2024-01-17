using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Util
{
    public static class TextureUtil
    {
        //Can be done better, I think
        public static Vector4[] GetPixelData(this Texture2D source, Rectangle sourceRect)
        {
            var dataSize = sourceRect.Width * sourceRect.Height;

            if (source.Format == SurfaceFormat.Color)
            {
                var pixels = new Color[dataSize];
                source.GetData(0, sourceRect, pixels, 0, dataSize);

                return pixels.Select((p) => p.ToVector4()).ToArray();
            }
            else if (source.Format == SurfaceFormat.Vector4)
            {
                var pixels = new Vector4[dataSize];
                source.GetData(0, sourceRect, pixels, 0, dataSize);
                return pixels;
            }
            else
                throw new ApplicationException($"Unsuported texture format {source.Format}");

        }

        public static void TransferPixels3d<T>(this Texture2D sourceTexture, T[] targetPixels, int targetW, int targetH, Rectangle sourceRect, int x, int y, int z) where T : struct
        {
            var rawData = sourceTexture.GetPixelData(sourceRect);
            var data = new T[rawData.Length];
            if (data is Vector4[] vArr)
            {
                rawData.CopyTo(vArr, 0);
            }
            else if (data is Color[] cArr)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    cArr[i] = new Color(rawData[i]);
                }
            }
            targetPixels.SetRectUnchecked3d(targetW, targetH, data, new Rectangle(
            x, y,
                sourceRect.Width, sourceRect.Height), z);
        }

        public static void SetData<T>(this Texture3D[] textures, T[][] pixels) where T : struct
        {
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i].SetData(pixels[i]);
            }
        }
        
    }
}

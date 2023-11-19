using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Audio.Sounds
{
    public static class SoundHelper
    {
        public static void ReadBuffer(byte[] targetBuffer, float[] sourceBuffer, int count, int targetOffset, int sourceOffset, int sourceFrequency)
        {
            for (int i = 0; i < count; i++)
            {
                var sample = (short)(sourceBuffer[i * sourceFrequency + sourceOffset] * short.MaxValue);
                targetBuffer[i * 2 + targetOffset + 0] = (byte)(sample & 255);
                sample >>= 8;
                targetBuffer[i * 2 + targetOffset + 1] = (byte)(sample & 255);
            }
        }

    }
}

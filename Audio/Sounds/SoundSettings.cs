using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Audio.Sounds
{
    public struct SoundSettings
    {
        public float volumeChangeRate;
        public VolumeMode volumeMode;
    }

    public enum VolumeMode
    {
        Linear = 0,
        Sqrt = 1
    }
}

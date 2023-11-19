using FMOD;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.FMOD_Audio
{
    public class FSoundInstance : IDisposable
    {
        internal EventInstance raw;

        private FSound sound;

        internal FSoundInstance(FSound sound)
        {
            this.sound = sound;
        }

        public void Start()
        {
            raw.start();
        }

        public void Stop()
        {
            raw.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void StopImmidiate()
        {
            raw.stop(STOP_MODE.IMMEDIATE);
        }

        public void Rewind()
        {
            raw.setTimelinePosition(0);
        }

        public void Seek(TimeSpan position)
        {
            var pos = (int)(position.TotalSeconds * sound.FSystem.SampleRate);
            raw.setTimelinePosition(pos);
        }

        public void Dispose()
        {
            raw.release().AssertOk();
        }
    }
}

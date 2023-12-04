using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.FMOD_Audio
{
    public class FSound : IDisposable
    {
        public TimeSpan Duration { get; private set; }
        public int Length => length;
        public string Path => path;
        
        private int length;
        private string path;

        internal FMODSystem FSystem { get; private set; }
        internal EventDescription raw;

        internal FSound(FMODSystem system)
        {
            this.FSystem = system;
        }

        internal void Init()
        {
            raw.getLength(out length).AssertOk();
            raw.getPath(out path).AssertOk();
            var sRate = (double)FSystem.SampleRate;

            Duration = TimeSpan.FromSeconds(length / sRate);

            
        }

        public FSoundInstance CreateInstance()
        {
            var instance = new FSoundInstance(this);
            raw.createInstance(out instance.raw).AssertOk();
            return instance;
        }

        public void Load()
        {
            raw.loadSampleData().AssertOk();
        }

        public void Dispose()
        {
            
        }
    }
}

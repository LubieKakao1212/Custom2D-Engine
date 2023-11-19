using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.FMOD_Audio
{
    public class FSoundBank : IDisposable
    {
        internal Bank raw;

        internal FMODSystem FSystem { get; private set; }

        private EventDescription[] eventsRaw;
        private FSound[] sounds;

        internal FSoundBank(FMODSystem sys)
        {
            this.FSystem = sys;
        }

        internal void Init()
        {
            raw.getEventList(out eventsRaw);
            sounds = eventsRaw.Select((raw) =>
            {
                var sound = new FSound(FSystem) { raw = raw };
                sound.Init();
                return sound;
            }).ToArray();
        }

        public FSound GetSound(string path)
        {
            return sounds.Where((sound) => sound.Path == path).First();
        }

        public void Dispose()
        {
            //TODO
            raw.unload();
        }
    }
}

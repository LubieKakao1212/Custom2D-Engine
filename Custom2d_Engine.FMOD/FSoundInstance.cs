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
        
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                raw.setPitch(value).AssertOk();
                pitch = value;
            }
        }
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                raw.setVolume(value).AssertOk();
                volume = value;
            }
        }
        public float Pan
        {
            get
            {
                return pan;
            }
            set
            {
                //var angle = value * MathF.PI / 2f;
                //FSoundHelper.Pan(angle, out var attribs);
                //raw.set3DAttributes(attribs).AssertOk();
                //raw.setParameterByID(parameterId_pan, value);
                pan = value;
            }
        }

        private float pan;

        internal PARAMETER_ID parameterId_pan;

        private float volume;
        private float pitch;
        private FSound sound;

        internal FSoundInstance(FSound sound)
        {
            this.sound = sound;

            //TODO if no parameter was found than it is not an error just no panning for this event
            //sound.raw.getParameterDescriptionByName("parameter:/Pan", out var panDesc).AssertOk();
            //parameterId_pan = panDesc.id;
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

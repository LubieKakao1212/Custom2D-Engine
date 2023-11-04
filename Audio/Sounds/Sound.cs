using Microsoft.Xna.Framework.Audio;
using System;

namespace Custom2d_Engine.Audio.Sounds
{
    public class Sound : IDisposable
    {
        public float Volume
        {
            get => mgSound.Volume;
            set => mgSound.Volume = value;
        }
        public float Pan
        {
            get => mgSound.Pan; 
            set => mgSound.Pan = value;
        }
        public float Pitch
        {
            get=> mgSound.Pitch;
            set => mgSound.Pitch = value;
        }

        protected SoundEffectInstance mgSound;

        internal Sound(SoundEffectInstance mgSound)
        {
            this.mgSound = mgSound;
        }

        ~Sound()
        {
            Dispose();
        }

        public void Play() => mgSound.Play();

        public void Pause() => mgSound.Pause();

        public virtual Sound Copy()
        {
            return null;
        }

        public virtual void Dispose()
        {
            mgSound.Dispose();
        }
    }
}

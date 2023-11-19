using Microsoft.Xna.Framework.Audio;
using System;

namespace Custom2d_Engine.Audio.Sounds
{
    public class SoundInstance : IDisposable
    {
        public virtual SoundState State => mgSound.State;
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
        public virtual bool IsLooped
        {
            get => mgSound.IsLooped;
            set => mgSound.IsLooped = value;
        }

        protected SoundEffectInstance mgSound;
        protected bool isMono;

        internal SoundInstance(SoundEffectInstance mgSound)
        {
            this.mgSound = mgSound;
        }

        ~SoundInstance()
        {
            Dispose();
        }

        public virtual void Play() => mgSound.Play();

        public virtual void Pause() => mgSound.Pause();

        public virtual void Stop() => mgSound.Stop();

        public virtual SoundInstance Copy()
        {
            return null;
        }

        public virtual void Dispose()
        {
            mgSound.Dispose();
        }
    }
}

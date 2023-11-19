using Custom2d_Engine.Audio.Sounds;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using NVorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Custom2d_Engine.Audio
{
    public class AudioManager : IDisposable
    {
        public string RootDirectory { get; set; } = "";

        private List<SoundInstance> sounds;

        private Dictionary<string, SoundEffect> soundEffectCache;
        private Dictionary<string, VorbisReader> soundReaderCache;

        public AudioManager() 
        { 
            sounds = new List<SoundInstance>();
            soundEffectCache = new Dictionary<string, SoundEffect>();
            soundReaderCache = new Dictionary<string, VorbisReader>();
        }

        public SoundInstance LoadOgg(string file)
        {
            file = Path.Combine(RootDirectory, file) + ".ogg";
            //Look for cached sounds
            if (soundEffectCache.TryGetValue(file, out var sound))
            {
                return new SoundInstance(sound.CreateInstance());
            }
            if(soundReaderCache.TryGetValue(file, out var reader))
            {
                return new StreamedSoundInstance(reader);
            }

            reader = new VorbisReader(file);

            var length = reader.TotalTime.TotalSeconds;

            if (length > 3f)
            { 
                soundReaderCache.Add(file, reader);
                return new StreamedSoundInstance(reader);
            }

            return AddCachedSound(file, reader);
        }

        private SoundInstance AddCachedSound(string file, VorbisReader reader)
        {
            var sampleCount = reader.TotalSamples;
            var channels = reader.Channels switch
            {
                1 => AudioChannels.Mono,
                2 => AudioChannels.Stereo,
                _ => throw new ArgumentException("channels")
            };
            var sampleRate = reader.SampleRate;

            var readBuffer = new float[sampleCount * (int)channels];
            var data = new byte[readBuffer.Length * 2];

            var count = reader.ReadSamples(readBuffer, 0, readBuffer.Length);

            if (count != readBuffer.Length)
            {
                throw new ApplicationException("Something went wrong");
            }

            SoundHelper.ReadBuffer(data, readBuffer, count, 0, 0, 1);

            var sound = new SoundEffect(data, sampleRate, channels);

            soundEffectCache.Add(file, sound);

            return new SoundInstance(sound.CreateInstance());
        }

        public void Dispose() 
        {
            foreach (var sound in sounds)
            {
                sound.Dispose();
            }
            foreach (var soundEffect in soundEffectCache.Values)
            {
                soundEffect.Dispose();
            }
            foreach (var reader in soundReaderCache.Values)
            {
                reader.Dispose();
            }
        }
    }
}

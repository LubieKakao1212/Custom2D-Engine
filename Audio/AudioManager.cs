using Custom2d_Engine.Audio.Sounds;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Audio
{
    public class AudioManager : IDisposable
    {
        public string RootDirectory { get; set; } = "";

        private List<Sound> sounds;

        public AudioManager() 
        { 
            sounds = new List<Sound>();
        }

        public OggSoundInstance LoadOgg(string file)
        {
            OggSoundInstance sound = new OggSoundInstance(System.IO.Path.Combine(RootDirectory, file) + ".ogg", true);

            sounds.Add(sound);

            return sound;
        }

        public void Dispose() 
        {
            foreach (var sound in sounds)
            {
                sound.Dispose();
            }
        }
    }
}

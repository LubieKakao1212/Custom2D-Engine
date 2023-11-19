using Custom2d_Engine.Util.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Audio.Sounds
{
    //TODO make file reading async
    public class StreamedSoundInstance : SoundInstance
    {
        public const int BuffersLoaded = 3;
        public const int MinBufferSamples = 8000;

        private readonly int TotalBufferSamples;

        public override SoundState State
        {
            get 
            {
                return state;
            }
        }
        public override bool IsLooped { get => isLooped; set => isLooped = value; }

        private AudioChannels channels;
        private long sampleCount;
        private bool isLooped;

        private VorbisReader reader;

        private DynamicSoundEffectInstance MgDynamicSound => (DynamicSoundEffectInstance)mgSound;
        
        private Queue<byte[]> buffers = new Queue<byte[]>();
        private float[] readBuffer;

        //We assume this sound is longer than 3 seconds
        private int BufferCountTillSoundEnd = -1;

        private SoundState state;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        internal StreamedSoundInstance(VorbisReader reader) : base(null)
        {
            this.reader = reader;

            var channels = reader.Channels;

            var sampleRate = reader.SampleRate;
            this.channels = (AudioChannels)channels;
            var sound = new DynamicSoundEffectInstance(sampleRate, this.channels);
            mgSound = sound;

            var sampleCount = reader.TotalSamples;
            this.sampleCount = sampleCount;

            sound.BufferNeeded += (_,_) => EnqueueBuffer();

            TotalBufferSamples = MathHelper.Max(MinBufferSamples, sampleRate) * channels;
            //We assume reader.TotalSamples does not exceed int.MaxValue because that would be a lot
            //TotalBufferSamples = (int)(TotalBufferSamples * 2);

            readBuffer = new float[TotalBufferSamples];

            //allLoaded = ;
            ReadBuffers();
            EnqueueBuffer();
            EnqueueBuffer();
        }

        public override void Play()
        {
            base.Play();
            state = SoundState.Playing;
        }

        public override void Pause()
        {
            base.Pause();
            state = SoundState.Paused;
        }

        public override void Stop()
        {
            base.Stop();
            state = SoundState.Stopped;
        }

        private void EnqueueBuffer()
        {
            MgDynamicSound.SubmitBuffer(buffers.Dequeue());
            ReadBuffer();
            if (BufferCountTillSoundEnd >= 0)
            {
                if (BufferCountTillSoundEnd == 0 && !isLooped)
                {
                    Stop();
                }
                BufferCountTillSoundEnd--;
            }  
        }

        private void ReadBuffers()
        {
            while (buffers.Count < BuffersLoaded)
            {
                ReadBuffer();
            }
        }

        private void ReadBuffer()
        {
            int samples = reader.ReadSamples(readBuffer, 0, TotalBufferSamples);
            var buffer = new byte[samples * 2];
            
            SoundHelper.ReadBuffer(buffer, readBuffer, samples, 0, 0, 1);

            buffers.Enqueue(buffer);

            Console.WriteLine("Read");

            if (reader.DecodedPosition == sampleCount)
            {
                reader.DecodedPosition = 0;
                BufferCountTillSoundEnd = buffers.Count;
                Console.WriteLine("Rewind");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            //Do not dispose the reader, AudioManager does it
            //reader.Dispose();
        }
    }
}

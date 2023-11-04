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
    public class OggSoundInstance : Sound
    {
        public const int BuffersLoaded = 3;
        public const int MinBufferSamples = 8000;

        private readonly int TotalBufferSamples;

        private VorbisReader reader;

        private DynamicSoundEffectInstance MgDynamicSound => (DynamicSoundEffectInstance)mgSound;

        private long sampleCount;

        private Queue<byte[]> buffers = new Queue<byte[]>();

        private float[] readBuffer;

        private AudioChannels channels;

        private bool streamed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="samplesPerSegment">false - load whole sound at once, true - stream the sound</param>
        internal OggSoundInstance(string fileName, bool streamed = false) : base(null)
        {
            reader = new VorbisReader(fileName);

            var channels = reader.Channels;

            var sampleRate = reader.SampleRate;
            this.channels = (AudioChannels)channels;
            var sound = new DynamicSoundEffectInstance(sampleRate, this.channels);
            mgSound = sound;

            var sampleCount = reader.TotalSamples;
            this.sampleCount = sampleCount;

            sound.BufferNeeded += (_,_) => EnqueueBuffer();

            this.streamed = streamed;
            if (streamed)
            {
                TotalBufferSamples = MathHelper.Max(MinBufferSamples, sampleRate) * channels;
            }
            else
            {
                //We assume reader.TotalSamples does not exceed int.MaxValue because that would be a lot
                TotalBufferSamples = (int)(reader.TotalSamples * 2);
            }
            readBuffer = new float[TotalBufferSamples];

            //allLoaded = ;
            ReadBuffers();
            EnqueueBuffer();
            EnqueueBuffer();
        }
        
        private void EnqueueBuffer()
        {
            if (streamed)
            {
                MgDynamicSound.SubmitBuffer(buffers.Dequeue());
                ReadBuffer();
            }
            else
            {
                MgDynamicSound.SubmitBuffer(buffers.Peek());
            }
        }

        private void ReadBuffers()
        {
            if (streamed)
            {
                while (buffers.Count < BuffersLoaded)
                {
                    ReadBuffer();
                }
            }
            else
            {
                ReadBuffer();
            }
        }

        private void ReadBuffer()
        {
            int samples = reader.ReadSamples(readBuffer, 0, TotalBufferSamples);
            var buffer = new byte[samples * 2];
            
            ReadBuffer(buffer, readBuffer, samples, 0, 0, 1);

            buffers.Enqueue(buffer);

            Console.WriteLine("Read");

            if (reader.DecodedPosition == sampleCount)
            {
                reader.DecodedPosition = 0;
                Console.WriteLine("Rewind");
            }
        }

        private void ReadBuffer(byte[] targetBuffer, float[] sourceBuffer, int count, int targetOffset, int sourceOffset, int sourceFrequency)
        {
            for (int i = 0; i < count; i++)
            {
                var sample = (short)(sourceBuffer[i * sourceFrequency + sourceOffset] * short.MaxValue);
                targetBuffer[i * 2 + targetOffset + 0] = (byte) (sample & 255);
                sample >>= 8;
                targetBuffer[i * 2 + targetOffset + 1] = (byte) (sample & 255);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            reader.Dispose();
        }
    }
}

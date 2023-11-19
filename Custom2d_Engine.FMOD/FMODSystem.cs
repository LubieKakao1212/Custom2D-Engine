using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.FMOD_Audio
{
    using FMOD;
    using FMOD.Studio;

    using StudioSystem = FMOD.Studio.System;
    using CoreSystem = FMOD.System;
    using System.IO;

    public class FMODSystem : IDisposable
    {
        private static bool instanceExists;

        public string RootDirectory { get; set; }

        public int SampleRate => sampleRate;
        public int sampleRate;

        private CoreSystem coreSys;
        private StudioSystem studioSys;
        private FSoundBank masterBank;
        private FSoundBank stringsBank;

        public FMODSystem()
        {
            if (instanceExists)
            {
                throw new ApplicationException("Attempting to create duplicate FMODSystem");
            }
            instanceExists = true;
            #region
            Factory.System_Create(out coreSys).AssertOk();
            coreSys.close().AssertOk();
            #endregion

            StudioSystem.create(out studioSys).AssertOk();
            studioSys.getCoreSystem(out coreSys).AssertOk();

            //coreSys.setSoftwareFormat(0, SPEAKERMODE.DEFAULT, 0);

            studioSys.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, nint.Zero);

            //TODO Temporary until proper setum is implemented
            coreSys.getSoftwareFormat(out sampleRate, out var _, out var _);
        }

        public void LoadMaster()
        {
            masterBank = LoadBank("Master");
            stringsBank = LoadBank("Master.strings");
        }

        public FSoundBank LoadBank(string path)
        {
            var bank = new FSoundBank(this);
            studioSys.loadBankFile(Path.Combine(RootDirectory, path) + ".bank", LOAD_BANK_FLAGS.NORMAL, out bank.raw).AssertOk();
            bank.Init();

            return bank;
        }

        public void Update()
        {
            studioSys.update();
        }
        
        public void Dispose()
        {
            instanceExists = false;

            studioSys.release();
            coreSys.release();
        }
    }
}

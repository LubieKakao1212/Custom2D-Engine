using System;
using System.IO;
using FMOD;
using FMOD.Studio;
using INITFLAGS = FMOD.Studio.INITFLAGS;

namespace Custom2d_Engine.FMOD_Audio;

using StudioSystem = FMOD.Studio.System;
using CoreSystem = FMOD.System;

public class FMODSystem : IDisposable {
    private static bool _instanceExists;

    public string RootDirectory { get; init; }

    public int SampleRate => sampleRate;
    public readonly int sampleRate;

    private CoreSystem _coreSys;
    private StudioSystem _studioSys;
    private FSoundBank _masterBank;
    private FSoundBank _stringsBank;

    public FMODSystem() {
        if (_instanceExists) {
            throw new ApplicationException("Attempting to create duplicate FMODSystem");
        }

        _instanceExists = true;

        #region

        Factory.System_Create(out _coreSys).AssertOk();
        _coreSys.close().AssertOk();

        #endregion

        StudioSystem.create(out _studioSys).AssertOk();
        _studioSys.getCoreSystem(out _coreSys).AssertOk();

        _studioSys.initialize(1024, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, nint.Zero);

        //TODO Temporary until proper setup is implemented
        _coreSys.getSoftwareFormat(out sampleRate, out var _, out var _);
    }

    public void LoadMaster() {
        _masterBank = LoadBank("Master");
        _stringsBank = LoadBank("Master.strings");
    }

    public FSoundBank LoadBank(string path) {
        var bank = new FSoundBank(this);
        _studioSys.loadBankFile(Path.Combine(RootDirectory, path) + ".bank", LOAD_BANK_FLAGS.NORMAL, out bank.raw)
            .AssertOk();
        bank.Init();

        return bank;
    }

    public void Update() {
        _studioSys.update();
    }

    public void Dispose() {
        _instanceExists = false;

        _studioSys.release();
        _coreSys.release();
    }
}
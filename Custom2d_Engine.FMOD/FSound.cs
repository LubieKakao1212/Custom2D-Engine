using System;
using FMOD.Studio;

namespace Custom2d_Engine.FMOD_Audio;

public class FSound : IDisposable {
    public TimeSpan Duration { get; private set; }
    public int Length => _length;
    public string Path => _path;

    private int _length;
    private string _path;

    internal FMODSystem FSystem { get; private set; }
    internal EventDescription raw;

    internal FSound(FMODSystem system) {
        FSystem = system;
    }

    internal void Init() {
        raw.getLength(out _length).AssertOk();
        raw.getPath(out _path).AssertOk();
        var sRate = (double)FSystem.SampleRate;

        Duration = TimeSpan.FromSeconds(_length / sRate);
    }

    public FSoundInstance CreateInstance() {
        var instance = new FSoundInstance(this);
        raw.createInstance(out instance.raw).AssertOk();
        return instance;
    }

    public void Load() {
        raw.loadSampleData().AssertOk();
    }

    public void Dispose() {
    }
}
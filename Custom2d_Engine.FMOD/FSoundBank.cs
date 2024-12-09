using System;
using System.Linq;
using FMOD.Studio;

namespace Custom2d_Engine.FMOD_Audio;

public class FSoundBank : IDisposable {
    internal Bank raw;

    internal FMODSystem FSystem { get; }

    private EventDescription[]? _eventsRaw;
    private FSound[]? _sounds;

    internal FSoundBank(FMODSystem sys) {
        FSystem = sys;
    }

    internal void Init() {
        raw.getEventList(out _eventsRaw);
        _sounds = _eventsRaw.Select(rawSound => {
            var sound = new FSound(FSystem) { raw = rawSound };
            sound.Init();
            return sound;
        }).ToArray();
    }

    public FSound? GetSound(string path) {
        return _sounds?.First(sound => sound.Path == path);
    }

    public void Dispose() {
        //TODO
        raw.unload();
    }
}
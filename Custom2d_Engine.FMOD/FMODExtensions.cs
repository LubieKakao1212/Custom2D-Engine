using System;
using FMOD;

namespace Custom2d_Engine.FMOD_Audio;

internal static class FMODExtensions {
    public static void AssertOk(this RESULT result) {
        if (result != RESULT.OK) {
            throw new ApplicationException($"Operation returned {result}, this is not OK",
                new Exception(Error.String(result)));
        }
    }
}
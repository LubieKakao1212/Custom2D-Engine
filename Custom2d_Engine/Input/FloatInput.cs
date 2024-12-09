using System;

namespace Custom2d_Engine.Input;

public class FloatInput : ContinuousInputBase<float> {
    public FloatInput(string name) : base(name, Epsilon) {
    }

    protected override float GetDistance(float value) {
        return MathF.Abs(value);
    }
}
using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Input.Binding;

//TODO
public class Vector2BindingInput : ValueInputBase<Vector2> {
    /// <summary>
    /// Temporary
    /// </summary>
    public const float Deadzone = 0.01f;

    public override string FriendlyName { get; }

    protected override Vector2 Value {
        get {
            var raw = new Vector2(_horizontal.GetCurrentValue<float>(), _vertical.GetCurrentValue<float>());
            var mag = raw.LengthSquared();
            if (mag > Deadzone) {
                return _normalize ? Vector2.Normalize(raw) : raw;
            }

            return Vector2.Zero;
        }
    }

    private readonly ValueInputBase<float> _horizontal;
    private readonly ValueInputBase<float> _vertical;

    private readonly bool _normalize;

    public Vector2BindingInput(string name, ValueInputBase<float> horizontal, ValueInputBase<float> vertical,
        bool normalize = false) {
        FriendlyName = name;
        this._horizontal = horizontal;
        this._vertical = vertical;
        this._normalize = normalize;
    }
}
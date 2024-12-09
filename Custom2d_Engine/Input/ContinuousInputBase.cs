namespace Custom2d_Engine.Input;

public abstract class ContinuousInputBase<T> : SettableValueInputBase<T> {
    public const float Epsilon = 1f / 64f;

    public override string FriendlyName { get; }

    protected readonly float epsilon;

    internal ContinuousInputBase(string name, float epsilon) {
        this.FriendlyName = name;
        this.epsilon = epsilon;
    }

    protected override bool IsActive(T value) {
        return GetDistance(value) > epsilon;
    }

    protected abstract float GetDistance(T value);
}
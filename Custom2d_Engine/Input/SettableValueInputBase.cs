namespace Custom2d_Engine.Input;

public abstract class SettableValueInputBase<T> : ValueInputBase<T> {
    protected override T Value => _value!;

    private bool _state;
    private T? _value;
    
    internal bool UpdateState(T newValue) {
        _value = newValue;
        var newState = IsActive(newValue);
        var changed = _state != newState;
        _state = newState;
        InvokeEvents(newState, changed);
        return changed;
    }

    protected abstract bool IsActive(T value);
}
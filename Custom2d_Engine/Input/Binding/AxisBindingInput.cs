using System;
using System.Collections.Generic;

namespace Custom2d_Engine.Input.Binding;

public class AxisBindingInput : ValueInputBase<float> {
    public override string FriendlyName => _name;

    protected override float Value {
        get {
            if (_binding == null) {
                //TODO Binding Exception
                throw new Exception("Unbound");
            }

            return (_binding.GetCurrentValue<bool>() ? _activeValue : _idleValue);
        }
    }

    private string _name;

    private ValueInputBase<bool>? _binding;

    private float _idleValue = 0, _activeValue = 1;

    public AxisBindingInput(string name) {
        this._name = name;
        _binding = null;
    }

    public AxisBindingInput Bind(ValueInputBase<bool> binding, bool inheritName = false) {
        UnbindCallbacks();
        _binding = binding;
        BindCallbacks();
        if (inheritName) {
            _name = binding.FriendlyName;
        }

        return this;
    }

    public AxisBindingInput SetValues(float idle, float active) {
        _idleValue = idle;
        _activeValue = active;
        return this;
    }

    private void UnbindCallbacks() {
        if (_binding != null) {
            _binding.Started -= PassStarted;
            _binding.Performed -= PassPerformed;
            _binding.Canceled -= PassCanceled;
        }
    }

    private void BindCallbacks() {
        if (_binding != null) {
            _binding.Started += PassStarted;
            _binding.Performed += PassPerformed;
            _binding.Canceled += PassCanceled;
        }
    }
}

//TODO May potentialy call Canceled and Started on the same frame
public class CompoundAxixBindingInput : ValueInputBase<float>, IBindingInput {
    public override string FriendlyName { get; }

    protected override float Value {
        get {
            var v = 0f;
            foreach (var binding in _bindings) {
                v += binding.GetCurrentValue<float>();
            }

            return v;
        }
    }

    private readonly HashSet<ValueInputBase<float>> _bindings;

    private int _activityState;
    private bool _updatedThisTick;

    public CompoundAxixBindingInput(string name) {
        this.FriendlyName = name;
        _bindings = new HashSet<ValueInputBase<float>>();
    }

    public CompoundAxixBindingInput Bind(ValueInputBase<float> binding) {
        if (_bindings.Add(binding)) {
            BindCallbacks(binding);
        }

        return this;
    }

    public CompoundAxixBindingInput UnBind(ValueInputBase<float> binding) {
        if (_bindings.Remove(binding)) {
            UnbindCallbacks(binding);
        }

        return this;
    }

    public void Update() {
        _updatedThisTick = false;
    }

    public void IncrementState(IInput _) {
        var changed = _activityState == 0;
        _activityState++;
        if (changed) {
            PassStarted(_);
        }
    }

    public void DecrementState(IInput _) {
        _activityState--;
        var changed = _activityState == 0;
        if (changed) {
            PassCanceled(_);
        }
    }

    private void Perform(IInput _) {
        if (!_updatedThisTick) {
            _updatedThisTick = true;
            PassPerformed(_);
        }
    }

    private void BindCallbacks(ValueInputBase<float> binding) {
        binding.Started += IncrementState;
        binding.Performed += Perform;
        binding.Canceled += DecrementState;
    }

    private void UnbindCallbacks(ValueInputBase<float> binding) {
        binding.Started -= IncrementState;
        binding.Performed -= Perform;
        binding.Canceled -= DecrementState;
    }
}
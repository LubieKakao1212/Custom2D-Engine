using System;

namespace Custom2d_Engine.Input.Binding {
    public class ProcessorInput<I, O> : ValueInputBase<O> {
        public override string FriendlyName => _name;

        protected override O Value {
            get {
                if (_input == null) {
                    //TODO Binding Exception
                    throw new Exception("Unbound");
                }

                return _processor(_input.GetCurrentValue<I>());
            }
        }

        private readonly Func<I, O> _processor;
        private ValueInputBase<I>? _input;
        private string _name;

        public ProcessorInput(Func<I, O> processor, string name) {
            this._processor = processor;
            this._name = name;
        }

        public ProcessorInput<I, O> Bind(ValueInputBase<I> binding, bool inheritName = false) {
            UnbindCallbacks();
            _input = binding;
            BindCallbacks();
            if (inheritName) {
                _name = binding.FriendlyName;
            }

            return this;
        }

        private void UnbindCallbacks() {
            if (_input != null) {
                _input.Started -= PassStarted;
                _input.Performed -= PassPerformed;
                _input.Canceled -= PassCanceled;
            }
        }

        private void BindCallbacks() {
            if (_input != null) {
                _input.Started += PassStarted;
                _input.Performed += PassPerformed;
                _input.Canceled += PassCanceled;
            }
        }
    }
}
using System;

namespace Custom2d_Engine.Input.Binding {
    public class UnboundInput : IInput {
        public static readonly UnboundInput Value = new UnboundInput();

        public string FriendlyName => "Unbound";

        public event Action<IInput> Started = delegate { };
        public event Action<IInput> Performed = delegate { };
        public event Action<IInput> Canceled = delegate { };

        public T GetCurrentValue<T>() {
            return default!;
        }
    }
}
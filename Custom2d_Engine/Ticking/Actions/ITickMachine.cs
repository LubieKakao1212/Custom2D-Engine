using System;

namespace Custom2d_Engine.Ticking.Actions {
    public interface ITickMachine {
        public bool Disposed { get; }

        public TimeSpan CurrentTime { get; }
        public TimeSpan Cooldown { get; }

        public void Forward(TimeSpan deltaTime);
    }
}
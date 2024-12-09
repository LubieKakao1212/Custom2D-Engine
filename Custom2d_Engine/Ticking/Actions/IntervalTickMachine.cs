using System;

namespace Custom2d_Engine.Ticking.Actions {
    public abstract class RepeatingTickMachineBase : TickMachineBase {
        private readonly Action _action;

        public RepeatingTickMachineBase(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(cooldown,
            phase) {
            this._action = action;
        }

        protected override bool Execute(TimeSpan deltaTime) {
            _action();
            RollBack();
            return true;
        }

        protected abstract void RollBack();
    }

    public class SimpleRepeatingTickMachine : RepeatingTickMachineBase {
        public SimpleRepeatingTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action,
            cooldown, phase) {
        }

        protected override void RollBack() {
            CurrentTime = TimeSpan.Zero;
        }
    }

    public class AccurateRepeatingTickMachine : RepeatingTickMachineBase {
        public AccurateRepeatingTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action,
            cooldown, phase) {
        }

        protected override void RollBack() {
            CurrentTime -= Cooldown;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Custom2d_Engine.Ticking.Actions {
    public class SequenceTickMachine : TickMachineBase {
        private readonly IEnumerator<TimeSpan> _action;

        public SequenceTickMachine(IEnumerator<TimeSpan> action, TimeSpan cooldown, TimeSpan phase = default) : base(
            cooldown, phase) {
            this._action = action;
        }

        protected override bool Execute(TimeSpan deltaTime) {
            CurrentTime -= Cooldown;
            if (!_action.MoveNext()) {
                Dispose();
                return false;
            }

            var value = _action.Current;
            if (value < TimeSpan.Zero) {
                Cooldown = TimeSpan.Zero;
                return false;
            }

            Cooldown = value;

            return true;
        }
    }
}
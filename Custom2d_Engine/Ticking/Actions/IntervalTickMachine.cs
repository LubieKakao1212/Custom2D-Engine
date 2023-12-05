using System;

namespace Custom2d_Engine.Ticking.Actions
{
    public abstract class RepeatingTickMachineBase : TickMachineBase
    {
        private Action action;

        public RepeatingTickMachineBase(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(cooldown, phase)
        {
            this.action = action;
        }

        protected override bool Execute(TimeSpan deltaTime)
        {
            action();
            RollBack();
            return true;
        }

        protected abstract void RollBack();
    }
    
    public class SimpleRepeatingTickMachine : RepeatingTickMachineBase
    {
        public SimpleRepeatingTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action, cooldown, phase) { }

        protected override void RollBack()
        {
            CurrentTime = TimeSpan.Zero;
        }
    }

    public class AccurateRepeatingTickMachine : RepeatingTickMachineBase
    {
        public AccurateRepeatingTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action, cooldown, phase) { }

        protected override void RollBack()
        { 
            CurrentTime -= Cooldown;
        }
    }
}

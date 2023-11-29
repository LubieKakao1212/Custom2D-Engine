using System;

namespace Custom2d_Engine.Ticking.Actions
{
    public abstract class IntervalTickMachineBase : TickMachineBase
    {
        private Action action;

        public IntervalTickMachineBase(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(cooldown, phase)
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
    
    public class SimpleIntervalTickMachine : IntervalTickMachineBase
    {
        public SimpleIntervalTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action, cooldown, phase) { }

        protected override void RollBack()
        {
            CurrentTime = TimeSpan.Zero;
        }
    }

    public class AccurateIntervalTickMachine : IntervalTickMachineBase
    {
        public AccurateIntervalTickMachine(Action action, TimeSpan cooldown, TimeSpan phase = default) : base(action, cooldown, phase) { }

        protected override void RollBack()
        { 
            CurrentTime -= Cooldown;
        }
    }
}

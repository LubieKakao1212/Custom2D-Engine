using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking.Actions
{
    public class SequenceTickMachine : TickMachineBase
    {
        private IEnumerator<TimeSpan> action;

        public SequenceTickMachine(IEnumerator<TimeSpan> action, TimeSpan cooldown, TimeSpan phase = default) : base(cooldown, phase)
        {
            this.action = action;
        }

        protected override bool Execute(TimeSpan deltaTime)
        {
            CurrentTime -= deltaTime;
            if(CurrentTime >= TimeSpan.Zero)
            {
                return true;
            }
            if (!action.MoveNext())
            {
                Dispose();
                return false;
            }
            var value = action.Current;
            if (value < TimeSpan.Zero)
            {
                Cooldown = TimeSpan.Zero;
                return false;
            }

            Cooldown = value;

            return true;
        }
    }
}

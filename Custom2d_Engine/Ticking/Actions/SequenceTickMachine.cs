using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking.Actions
{
    public class SequenceTickMachine : TickMachineBase
    {
        private IEnumerator<float> action;

        public SequenceTickMachine(IEnumerator<float> action, TimeSpan cooldown, TimeSpan phase = default) : base(cooldown, phase)
        {
            this.action = action;
        }

        protected override bool Execute(TimeSpan deltaTime)
        {
            CurrentTime -= Cooldown;

            if (!action.MoveNext())
            {
                return false;
            }
            var value = action.Current;
            if (value < 0)
            {
                return false;
            }

            Cooldown = TimeSpan.FromSeconds(value);

            return true;
        }
    }
}

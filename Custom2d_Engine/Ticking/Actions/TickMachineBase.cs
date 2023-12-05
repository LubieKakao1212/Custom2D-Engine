using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking.Actions
{
    public abstract class TickMachineBase : ITickMachine
    {
        public TimeSpan CurrentTime { get; protected set; }
        public TimeSpan Cooldown { get; protected set; }

        public bool Disposed { get; protected set; } = false;

        public TickMachineBase(TimeSpan cooldown, TimeSpan phase = default)
        {
            this.Cooldown = cooldown;
            this.CurrentTime = phase;
        }

        public void Forward(TimeSpan deltaTime)
        {
            CurrentTime += deltaTime;
            do
            {
                if (CurrentTime < Cooldown)
                {
                    break;
                }
            }
            while (Execute(deltaTime));
        }

        public void Dispose()
        {
            Disposed = true;
        }

        protected abstract bool Execute(TimeSpan deltaTime);
    }
}

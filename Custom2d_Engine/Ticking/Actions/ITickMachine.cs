using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking.Actions
{
    public interface ITickMachine
    {
        public bool Disposed { get; }

        public TimeSpan CurrentTime { get; }
        public TimeSpan Cooldown { get; }

        public void Forward(TimeSpan deltaTime);
    }
}

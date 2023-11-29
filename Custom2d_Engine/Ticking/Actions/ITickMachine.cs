using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking.Actions
{
    public interface ITickMachine
    {
        public TimeSpan CurrentTime { get; }
        public TimeSpan Cooldown { get; }

        public void Forward(TimeSpan deltaTime);
    }
}

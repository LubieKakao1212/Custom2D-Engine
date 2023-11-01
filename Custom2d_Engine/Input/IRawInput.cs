using Custom2d_Engine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input
{
    //Unuseds
    public interface IRawInput : IInput
    {
        public InputDevice OriginDevice { get; }
    }
}

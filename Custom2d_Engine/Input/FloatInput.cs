using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input
{
    public class FloatInput : ContinousInputBase<float>
    {
        public FloatInput(string name) : base(name, Epsilon)
        {
            
        }

        protected override float GetDistance(float value)
        {
            return MathF.Abs(value);
        }
    }
}

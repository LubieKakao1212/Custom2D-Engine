using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Util
{
    public class Reference<T> where T : struct
    {
        public T Value { get; set; }

        public Reference(T initialValue = default) { Value = initialValue; }

        public static implicit operator T(Reference<T> reference) 
        {
            return reference.Value;
        }
    }
}

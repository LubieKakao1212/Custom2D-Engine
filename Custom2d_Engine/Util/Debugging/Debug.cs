using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Util.Debugging
{
    public static class Debug
    {
        public static T LogThis<T>(this T value, string prefix = "") where T : struct
        {
            Console.WriteLine(prefix + value);

            return value;
        }
    }
}

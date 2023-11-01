using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input
{
    public class KeyInput : BoolInput
    {
        public Keys Key { get; }

        internal KeyInput(Keys key) : base(key.ToString())
        {
            Key = key;
        }
    }
}

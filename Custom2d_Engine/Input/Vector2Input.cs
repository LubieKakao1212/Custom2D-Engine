using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input
{
    public class Vector2Input : ContinousInputBase<Vector2>
    {
        public Vector2Input(string name) : base(name, Epsilon * Epsilon)
        {
        }

        protected override float GetDistance(Vector2 value)
        {
            return value.LengthSquared();
        }
    }
}

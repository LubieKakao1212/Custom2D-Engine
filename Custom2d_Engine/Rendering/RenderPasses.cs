using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering
{
    public enum RenderPasses : byte
    {
        Normals = 0,
        Lights = 1,
        Final = 2,
    }
}

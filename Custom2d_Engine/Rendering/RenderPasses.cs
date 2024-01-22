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

    public static class RenderPassesExtensions
    {
        public static int GetShaderPasssIdx(this RenderPasses pass) => pass switch
        {
            RenderPasses.Normals => 0,
            RenderPasses.Lights => -1,
            RenderPasses.Final => 1,
            _ => -1
        };
    }
}

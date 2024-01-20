using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering
{
    public enum QueueBehaviour
    {
        BatchRender = 0,
        Skip = 1,
        Interupt = 3,
        CustomDraw = 4
    }
}

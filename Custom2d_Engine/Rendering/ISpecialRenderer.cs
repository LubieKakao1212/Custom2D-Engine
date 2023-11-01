using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Rendering
{
    public interface ISpecialRenderer
    {
        void Render(Camera camera);
    }
}

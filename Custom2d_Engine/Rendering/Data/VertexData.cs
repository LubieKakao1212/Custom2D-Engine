using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace Custom2d_Engine.Rendering.Data {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2DPosition {
        public static VertexDeclaration VertexDeclaration { get; private set; }

        public Vector2 Pos;

        public Vertex2DPosition(Vector2 pos) {
            Pos = pos;
        }

        static Vertex2DPosition() {
            VertexDeclaration =
                new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position,
                    0));
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace MarchingSquares.MarchingSquares.Rendering {
    /// <summary>
    /// Unuseds
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GridDepthVertex {
        public Vector3 positionDepth;
        public Vector2 UV;

        public static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
    }
}
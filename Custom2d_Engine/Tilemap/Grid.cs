using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using Custom2d_Engine.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Tilemap
{
    public class Grid : HierarchyObject
    {
        public Vector2 CellSize;
        
        public Grid(Vector2 cellSize)
        {
            CellSize = cellSize;
        }

        public Point WorldToCell(Vector2 worldPos) {
            var localPos = Transform.WorldToLocal.TransformPoint(worldPos);
            localPos += Vector2.One;
            localPos /= 2f;
            
            return LocalToCell(localPos);
        }

        public Point LocalToCell(Vector2 localPos)
        {
            localPos /= CellSize;
            
            return localPos.FloorToInt();
        }

        public Vector2 GridToCellCornerLocal(Point gridPos)
        {
            return new Vector2(gridPos.X * CellSize.X, gridPos.Y * CellSize.Y);
        }

        public Vector2 GridToCellCornerWorld(Point gridPos) {
            //TODO make sure this works
            var localPos = GridToCellCornerLocal(gridPos);
            localPos *= 2f;
            localPos -= Vector2.One;
            
            return Transform.LocalToWorld.TransformPoint(localPos);
        }

        public Vector2 GridToCellCenterLocal(Point gridPos)
        {
            return GridToCellCornerLocal(gridPos) + (CellSize / 2f);
        }

        public Vector2 GridToCellCenterWorld(Point gridPos) {
            //TODO make sure this works
            var localPos = GridToCellCenterLocal(gridPos);
            localPos *= 2f;
            localPos -= Vector2.One;
            return Transform.LocalToWorld.TransformPoint(localPos);
        }

    }
}

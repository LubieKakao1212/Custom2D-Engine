using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using Custom2d_Engine.Scenes;

namespace Custom2d_Engine.Tilemap {
    public class Grid : HierarchyObject {
        public Vector2 cellSize;

        public Grid(Vector2 cellSize) {
            this.cellSize = cellSize;
        }

        public Point WorldToCell(Vector2 worldPos) {
            var localPos = Transform.WorldToLocal.TransformPoint(worldPos);

            return LocalToCell(localPos);
        }

        public Point LocalToCell(Vector2 localPos) {
            localPos /= cellSize;

            return localPos.FloorToInt();
        }

        public Vector2 GridToCellCornerLocal(Point gridPos) {
            return new Vector2(gridPos.X * cellSize.X, gridPos.Y * cellSize.Y);
        }

        public Vector2 GridToCellCornerWorld(Point gridPos) {
            return Transform.LocalToWorld.TransformPoint(GridToCellCornerLocal(gridPos));
        }

        public Vector2 GridToCellCenterLocal(Point gridPos) {
            return GridToCellCornerLocal(gridPos) + (cellSize / 2f);
        }

        public Vector2 GridToCellCenterWorld(Point gridPos) {
            return Transform.LocalToWorld.TransformPoint(GridToCellCenterLocal(gridPos));
        }
    }
}
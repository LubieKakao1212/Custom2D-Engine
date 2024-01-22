using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;
using System;
using System.Data;

namespace Custom2d_Engine.Scenes
{
    public class Camera : HierarchyObject
    {
        /*public BoundingRect WorldBounds 
        { 
            get
            {
                if (!worldBounds.HasValue)
                {
                    worldBounds = (Transform.LocalToWorld )).Inverse();
                }
                return worldBounds.Value;
            }
        }*/

        public static BoundingRect CullingRect { get; } = BoundingRect.Normal
            //For debugging
            //.Scaled(0.2f)
            ;

        public TransformMatrix ProjectionMatrix
        {
            get
            {
                if (!projectionMatrix.HasValue)
                {
                    projectionMatrix = (Transform.LocalToWorld * Matrix2x2.Scale(new Vector2(ViewSize * aspectRatio, ViewSize))).Inverse();
                }
                return projectionMatrix.Value;
            }
        }

        public float ViewSize { get => viewSize; set { viewSize = value; projectionMatrix = null; } }

        public float AspectRatio { get => aspectRatio; set { aspectRatio = value; projectionMatrix = null; } }

        private TransformMatrix? projectionMatrix;
        private BoundingRect? worldBounds;
        private float viewSize = 1f;
        private float aspectRatio = 1f;

        public Camera()
        {
            Transform.Changed += () =>
            {
                projectionMatrix = null;
                worldBounds = null;
            };
        }

        public bool Cull(BoundingRect rect)
        {
            rect = rect.Transformed(ProjectionMatrix);

            return rect.Intersects(CullingRect);
        }

        public static BoundingRect CullReverse(in TransformMatrix projectionMatrix, in TransformMatrix WtL)
        {
            var VtW = projectionMatrix.Inverse();
            //First View to World than World to (Object)Local
            var VtL = WtL * VtW;

            var rect = CullingRect;
            rect.Transform(VtL);
            return rect;
        }

        public Vector2 ViewToWorldPos(Vector2 viewPos)
        {
            //TODO Cache inverse?
            return ProjectionMatrix.Inverse().TransformPoint(viewPos);
        }
    }
}

using Custom2d_Engine.Physics;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledLib;
using TiledLib.Objects;

namespace Custom2d_Engine.TMX.LayerProcessor
{
    using static ObjectLayerProcessors;

    public static class ObjectLayerProcessorsPhysics
    {
        /// <summary>
        /// Usefull for static world collisions, for "place and forget"
        /// </summary>
        public static Func<LoadedMap<Color>, RectangleObject, bool> AsBoxCollider(PhysicsBodyObject targetObject)
        {
            return (map, rectangle) => 
            {
                var body = targetObject.PhysicsBody;
                var rect = PolygonTools.CreateRectangle(1f, 1f);
                rect.Translate(new Vector2(1f, -1f));
                rect.Scale(new Vector2((float)rectangle.Width, (float)rectangle.Height));
                //TODO Rotation
                rect.Rotate(MathHelper.ToRadians(-318.56f));
                rect.Translate(rectangle.CorrectPosition(map.Height));
                body.CreatePolygon(rect, 1f);
                return true;
            };
        }

        /// <summary>
        /// Usefull for static world collisions, for "place and forget"
        /// </summary>
        public static Func<LoadedMap<Color>, PolyLineObject, bool> AsLineCollider(PhysicsBodyObject targetObject)
        {
            return (map, polyline) =>
            {
                var body = targetObject.PhysicsBody;
                body.CreateChainShape(polyline.Polyline.Shape(polyline, map.Height));
                return true;
            };
        }

        /// <summary>
        /// Usefull for static world collisions, for "place and forget"
        /// </summary>
        public static Func<LoadedMap<Color>, PolygonObject, bool> AsLineLoopCollider(PhysicsBodyObject targetObject)
        {
            return (map, polygon) =>
            {
                var body = targetObject.PhysicsBody;
                body.CreateLoopShape(polygon.Polygon.Shape(polygon, map.Height));
                return true;
            };
        }

        /// <summary>
        /// Usefull for static world collisions, for "place and forget"
        /// </summary>
        public static Func<LoadedMap<Color>, PolygonObject, bool> AsPolygonCollider(PhysicsBodyObject targetObject)
        {
            return (map, polygon) =>
            {
                var body = targetObject.PhysicsBody;
                body.CreatePolygon(polygon.Polygon.Shape(polygon, map.Height), 1f);
                return true;
            };
        }

        /// <summary>
        /// Usefull for static world collisions, for "place and forget"
        /// </summary>
        public static Func<LoadedMap<Color>, EllipseObject, bool> AsEllipseCollider(PhysicsBodyObject targetObject, int ellipseEdgeCount = 16, bool forceEllipse = false)
        {
            return (map, ellipse) =>
            {
                var body = targetObject.PhysicsBody;
                var w = ellipse.Width;
                var h = ellipse.Height;
                Vector2 offset = ellipse.CorrectPosition(map.Height);
                if (!forceEllipse && w == h)
                {
                    body.CreateCircle((float)w, 1f, offset);
                }
                else
                {
                    //body.CreateEllipse(,);
                }
                return true;
            };
        }

        public static LoadedMap<Color>.ObjectLayerHandler ShapesToCollisions(PhysicsBodyObject targetObject)
        {
            return FirstFrom(
                OfType(AsBoxCollider(targetObject)),
                OfType(AsLineCollider(targetObject)),
                OfType(AsEllipseCollider(targetObject)),
                OfType(
                    FirstFrom(
                        WithProperty(AsPolygonCollider(targetObject),"fill", "true"),
                        AsLineLoopCollider(targetObject)
                        )
                    )
                ).Build<BaseObject>();
        }

        private static Vertices Shape(this Position[] points, BaseObject obj, int mapHeight)
        {
            var line = new Vertices(points.ToMonogame());
            //TODO Rotation
            //rect.Rotate(rectangle.);
            line.Translate(obj.CorrectPosition(mapHeight));
            return line;
        }

        private static Vector2 CorrectPosition(this BaseObject obj, int mapHeight)
        {
            return new Vector2((float)obj.X, mapHeight - (float)obj.Y) * 2f + new Vector2(-1f, 1f);
        }

        private static IEnumerable<Vector2> ToMonogame(this Position[] points)
        {
            return points.Select((p) => new Vector2((float)p.X, -(float)p.Y) * 2f).ToArray();
        }

    }
}

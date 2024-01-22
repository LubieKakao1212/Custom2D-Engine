using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Scenes.Drawable;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes.Factory
{
    public static partial class HierarchyFactory
    {
        public static HierarchyObject HierarchyObject()
        {
            return new HierarchyObject();
        }

        public static Func<HierarchyObject> HierarchyObjectFactory()
        {
            return HierarchyObject;
        }

        public static Func<DrawableObject> DrawableObjectFactory(Sprite sprite, Color? color = null, float drawOrder = 0f)
        {
            return () => new DrawableObject(color.GetValueOrDefault(Color.White), drawOrder) { Sprite = sprite };
        }

        public static Func<OffsetObject> OffsetObjectFactory(Vector2? offset = default, float? rotation = 0f)
        {
            return () => new OffsetObject() { GlobalOffset = offset, Rotation = rotation };
        }
    }
}

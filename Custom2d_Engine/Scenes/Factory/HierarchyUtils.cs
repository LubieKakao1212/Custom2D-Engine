using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering.Sprites;
using Microsoft.Xna.Framework;
using System;

namespace Custom2d_Engine.Scenes
{
    public static class HierarchyUtils
    {
        /// <summary>
        /// Creates a new object as a child of this object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor">Should always return a new nstance of the child object</param>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="localScale"></param>
        public static T CreateChild<T>(this HierarchyObject thisObj, Func<T> constructor, Vector2 localPosition = default, float localRotation = 0f, Vector2? localScale = default) where T : HierarchyObject
        {
            var child = constructor();
            child.Parent = thisObj;
            child.Transform.LocalPosition = localPosition;
            child.Transform.LocalRotation = localRotation;
            child.Transform.LocalScale = localScale.GetValueOrDefault(Vector2.One);
            return child;
        }

        /// <inheritdoc cref="CreateChild{T}(HierarchyObject, Func{T}, Vector2, float, Vector2?)"/>
        /// <typeparam name="T"></typeparam>
        /// <param name="childOut">child created, usefull for keeping the reference while chaining child creation</param>
        /// <returns>same as <paramref name="childOut"/></returns>
        public static T CreateChild<T>(this HierarchyObject thisObj, Func<T> constructor, out T childOut, Vector2 localPosition = default, float localRotation = 0f, Vector2? localScale = default) where T : HierarchyObject
        {
            childOut = thisObj.CreateChild(constructor, localPosition, localRotation, localScale);
            return childOut;
        }
    }

    namespace Factory
    {
        public static class HierarchyUtils
        {
            public static HierarchyObject CreateChild(this HierarchyObject thisObj, Vector2 localPosition = default, float localRotation = 0f, Vector2? localScale = default)
            {
                return thisObj.CreateChild(HierarchyFactory.HierarchyObjectFactory(), localPosition, localRotation, localScale);
            }

            public static DrawableObject CreateDrawableChild(this HierarchyObject thisObj, Sprite sprite, Vector2 localPosition = default, float localRotation = 0f, Vector2? localScale = default, Color? color = null, float drawOrder = 0f)
            {
                return thisObj.CreateChild(HierarchyFactory.DrawableObjectFactory(sprite, color, drawOrder), localPosition, localRotation, localScale);
            }

            public static OffsetObject CreateOffsetObject(this HierarchyObject thisObj, Vector2 offset = default, float rotation = 0f,
               Vector2? localScale = default, TransformSpace positionSpace = TransformSpace.Local, TransformSpace rotationSpace =  TransformSpace.Local)
            {
                var pLocal = positionSpace == TransformSpace.Local;
                var rLocal = rotationSpace == TransformSpace.Local;

                var localPos = pLocal ? offset : Vector2.Zero;
                Vector2? localOffset = pLocal ? null : offset;
                var localRotation = rLocal ? rotation : 0f;
                float? globalRotation = rLocal ? null : rotation;

                return thisObj.CreateChild(
                    HierarchyFactory.OffsetObjectFactory(localOffset, globalRotation), 
                    localPos,
                    localRotation,
                    localScale
                    );
            }
        }
    }
}

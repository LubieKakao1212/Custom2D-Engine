using Microsoft.Xna.Framework;
using Custom2d_Engine.Rendering.Sprites;

namespace Custom2d_Engine.Scenes {
    using Math;

    public class DrawableObject : HierarchyObject {
        public Color Color { get; set; }
        public float DrawOrder { get; set; }

        public Sprite Sprite { get; set; } = new Sprite()
            { TextureIndex = 0, TextureRect = new BoundingRect(Vector2.Zero, Vector2.Zero) };

        /// <summary>
        /// Can this object be batched with other Objects?
        /// </summary>
        public bool InteruptQueue { get; protected set; }

        public DrawableObject(Color color, float drawOrder) {
            Color = color;
            DrawOrder = drawOrder;
        }

        public DrawableObject SetInterruptQueue(bool interruptQueue) {
            InteruptQueue = interruptQueue;
            return this;
        }
    }
}
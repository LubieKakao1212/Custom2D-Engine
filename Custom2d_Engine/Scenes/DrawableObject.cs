using Microsoft.Xna.Framework;
using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Scenes.Events;

namespace Custom2d_Engine.Scenes
{
    using Custom2d_Engine.Rendering;
    using Math;
    using System;

    public class DrawableObject : HierarchyObject
    {
        public Color Color { get; set; }
        public float DrawOrder { get; set; }
        public long DrawLayerMask => drawLayerMask;
        
        public Sprite Sprite { get; set; } = new Sprite() { TextureIndex = 0, TextureRect = new BoundingRect(Vector2.Zero, Vector2.Zero) };

        /// <summary>
        /// Controls queue behaviour for this object
        /// </summary>
        protected internal QueueBehaviour[] PassQueueBehaviours;
        
        //All
        private long drawLayerMask = -1;

        public DrawableObject(Color color, float drawOrder) : base()
        {
            Color = color;
            DrawOrder = drawOrder;
            PassQueueBehaviours = new QueueBehaviour[3];
            SetQueueBehaviour(RenderPasses.Normals, QueueBehaviour.BatchRender);
            SetQueueBehaviour(RenderPasses.Lights, QueueBehaviour.Skip);
            SetQueueBehaviour(RenderPasses.Final, QueueBehaviour.BatchRender);
        }

        public virtual DrawableObject SetQueueBehaviour(RenderPasses pass, QueueBehaviour behaviour)
        {
            if (behaviour == QueueBehaviour.CustomDraw)
            {
                throw new ArgumentException($"Attempting to set {nameof(QueueBehaviour)} of non-special object to {nameof(QueueBehaviour.CustomDraw)}");
            }
            this.PassQueueBehaviours[(byte)pass] = behaviour;
            return this;
        }
    }
}

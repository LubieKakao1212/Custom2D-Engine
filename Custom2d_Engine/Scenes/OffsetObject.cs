using Custom2d_Engine.Math;
using Custom2d_Engine.Scenes.Events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Scenes
{
    /// <summary>
    /// Used for keeping a global-space offset between two objects as well as keeping global rotation
    /// </summary>
    public class OffsetObject : HierarchyObject, IUpdatable
    {
        public float Order { get; }

        /// <summary>
        /// If not null will keep objects global position at parent global position + this value
        /// </summary>
        public Vector2? GlobalOffset { get; set; } = null;
        
        /// <summary>
        /// If not null will keep objects global rotation at that value
        /// </summary>
        public float? Rotation { get; set; } = null;

        public OffsetObject() 
        {

        }

        public void Update(GameTime time)
        {
            if (GlobalOffset.HasValue)
            {
                Transform.GlobalPosition = Parent.Transform.GlobalPosition + GlobalOffset.Value;
            }

            if (Rotation.HasValue)
            {
                Transform.GlobalRotation = Rotation.Value;
            }
        }
    }
}
